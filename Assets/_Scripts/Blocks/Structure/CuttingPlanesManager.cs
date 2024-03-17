using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	
	public class CuttingPlanesManager : SubcontainerModule, ICutPlanesDataProvider
	{
        // Creating a imaginary planes using axle(CuttingPlanePosition.Face) and a point on it(CuttingPlanePosition.Coordinate)
        // resulting plane will contain pins of different details, so when you placing a new brick,
        // it will be connected to all details it cover.
        // so we can just set the initial point and the system connect new detail to all blocks automatically

        private int _nextCuttingPlaneId = 1;
        private Dictionary<CuttingPlanePosition, ICuttingPlane> _cuttingPlanes = new();
		private Dictionary<int, CuttingPlaneLockZone> _lockZones = new();
		private Dictionary<int, ICuttingPlane> _cuttingPlanesById = new();
		private readonly ComplexResolver<IBlocksHost, PlacedBlocksListHandler, BlockStructureModule> _localResolver;
		protected IBlocksHost BlocksHost => _localResolver.Item1;
		protected PlacedBlocksListHandler BlocksList => _localResolver.Item2;
		protected BlockStructureModule BlocksStructure => _localResolver.Item3;
		public System.Action<CuttingPlanePosition, CuttingPlaneLockZone> OnLockedZonesChangedEvent;

		public CuttingPlanesManager(Container container) : base(container)
		{
			_localResolver = new(OnLocalDependenciesResolved, container);
			_localResolver.CheckDependencies();
		}
		private void OnLocalDependenciesResolved()
		{
            var blocks = BlocksList.GetPlacedBlocks();
            if (blocks.Count > 0) foreach (var block in blocks) OnBlockAdded(block);
            BlocksHost.OnBlockPlacedEvent += OnBlockAdded;

            var connections = BlocksStructure.GetConnections();
            if (connections.Count > 0)
            {
                foreach (var connection in connections) OnConnectionCreated(connection);
            }
            BlocksStructure.OnConnectionCreatedEvent += OnConnectionCreated;
        }
		private void AddCuttingPlane(CuttingPlanePosition coordinate, ICuttingPlane plane)
		{
			_cuttingPlanes.Add(coordinate, plane);
			_cuttingPlanesById.Add(plane.ID, plane);
		}

		private void OnBlockAdded(PlacedBlock block)
		{
			var planesContainer = block.Properties.GetPlanesList();
			var planes = planesContainer.Planes;
			for (byte i = 0; i < planes.Count; i++)
			{
				AddFitPlane(i, planes[i], block);
			}
		}
		private void OnConnectionCreated(BlocksConnection connection)
		{
			var lockedPins = connection.LockedPins;
            AddLockZones(lockedPins.CutPlaneA_id, lockedPins.GetLockedPinsA());
            AddLockZones(lockedPins.CutPlaneB_id, lockedPins.GetLockedPinsB());
        }
		private void AddFitPlane(byte subPlaneId, FitPlaneConfig fitPlaneConfig, PlacedBlock block)
		{
			BlockFaceDirection face = fitPlaneConfig.Face;
			Vector3 normal = face.Normal;
			Vector3 zeroPosInModelSpace = fitPlaneConfig.ZeroPos;

			float coordinate = Utilities.DefineCutPlaneCoordinate(zeroPosInModelSpace, normal);
			ICuttingPlane cuttingPlane;

			var dataProvider = fitPlaneConfig.CreateDataProvider(block.ID,subPlaneId,block, face);
			var key = new CuttingPlanePosition(face, coordinate);

			if (!_cuttingPlanes.TryGetValue(key, out cuttingPlane))
			{
				cuttingPlane = new OneItemCuttingPlane(_nextCuttingPlaneId++, dataProvider, face, coordinate);
				AddCuttingPlane(key, cuttingPlane);
			}
			else _cuttingPlanes[key] = cuttingPlane.AddFitPlaneProvider(dataProvider);
			//Debug.Log(key);
		}
		public void AddLockZones(int cutPlaneID, IReadOnlyCollection<ConnectingPin> lockedPins)
		{
			var cuttingPlane = _cuttingPlanesById[cutPlaneID];
            CuttingPlaneLockZone zone;
            if (!_lockZones.TryGetValue(cutPlaneID, out zone))
            {
                zone = new CuttingPlaneLockZone(cuttingPlane);
                _lockZones.Add(cutPlaneID, zone);
            }
			zone.AddLockedPins(lockedPins);
			OnLockedZonesChangedEvent?.Invoke(cuttingPlane.ToCoordinate(), zone);
        }
		public void RemoveLocks(int cutPlaneID, IReadOnlyCollection<ConnectingPin> pins)
		{
            if (_lockZones.TryGetValue(cutPlaneID, out var zone))
            {
				if (zone.RemoveLockedPins(pins))
				{
					_lockZones.Remove(cutPlaneID);
					OnLockedZonesChangedEvent?.Invoke(_cuttingPlanesById[cutPlaneID].ToCoordinate(), null);
                }
				else
				{
                    OnLockedZonesChangedEvent?.Invoke(_cuttingPlanesById[cutPlaneID].ToCoordinate(), zone);
                }
            }
        }

        public bool TryGetCuttingPlane(CuttingPlanePosition coord, out ICuttingPlane cuttingPlane) => _cuttingPlanes.TryGetValue(coord, out cuttingPlane);
        public bool TryGetCuttingPlane(FitElementStructureAddress address, out ICuttingPlane plane) => _cuttingPlanesById.TryGetValue(address.CutPlaneID, out plane);
        public bool TryGetCuttingPlane(PlacedBlock block, BlockFaceDirection localDirection, out ICuttingPlane plane)
        {
            var coords = Utilities.DefineCutPlaneCoordinate(block, localDirection);
            return _cuttingPlanes.TryGetValue(coords, out plane);
        }
        public bool TryGetFitElementPosition(Vector3 localPos,Vector3 normal, PlacedBlock block, out FoundedFitElementPosition fitPosition)
		{
            var face = new BlockFaceDirection(normal);
            float coordinate = Utilities.DefineCutPlaneCoordinate(localPos, face.Normal);
			if (_cuttingPlanes.TryGetValue(new(face, coordinate), out var cuttingPlane))
			{
				Vector2 cutPlanePos = face.LocalToFaceDirection(localPos);
				//Debug.Log($"{localPos} -> {cutPlanePos}");
				if (cuttingPlane.TryDefineFitPlane(cutPlanePos, out IFitPlaneDataProvider fitPlane) && fitPlane.TryGetPinPosition(cutPlanePos, out var planeAddress))
				{
					var address = new FitElementStructureAddress(block.ID, cuttingPlane.ID, face, planeAddress);
					
					var facePoint = fitPlane.GetFitElementFaceVirtualPoint(address.PlaneAddress.PinIndex);					
					var localPoint = new VirtualPoint(cuttingPlane.CutPlaneToLocalPos(facePoint.Position), cuttingPlane.Face.ToRotation() * facePoint.Rotation);
					var worldPoint = new VirtualPoint(BlocksHost.TransformPosition(localPoint.Position), BlocksHost.ModelsHost.rotation * localPoint.Rotation);

					bool isObstructed = false;
					//Debug.Log(landingRectangle.Rect.position);
					if (_lockZones.TryGetValue(cuttingPlane.ID, out var lockZone))
					{
                        if (lockZone.Contains(block.ID, address.PlaneAddress))
                        {
                            isObstructed = true;
                        }
                    }
					

                    fitPosition = new FoundedFitElementPosition(BlocksHost.ID, address, worldPoint, normal, isObstructed);
                    return true;
				}
			}
			//else Debug.Log(coordinate);
			fitPosition = default;
			return false;
        }
        public bool TryConnectNewBlock(PlacedBlock blockBase, FitElementStructureAddress address, VirtualBlock planningBlock, out ConnectedAndLockedPinsContainer pinsContainer)
		{
			var key = Utilities.DefineCutPlaneCoordinate(blockBase,address.ContactFace);
			if (_cuttingPlanes.TryGetValue(key, out var cuttingPlane) )
			{
				var landingRectangle = Utilities.ProjectBlock(cuttingPlane.Face, planningBlock);
				//Debug.Log(landingRectangle);

                var landingPins = cuttingPlane.GetLandingPinsList(landingRectangle);
				var connectFace = cuttingPlane.Face.Inverse().Rotate(Quaternion.Inverse(planningBlock.Rotation));
				var newBlockPins = planningBlock.Properties.GetPlanesList().CreateLandingPinsList(-1,planningBlock, connectFace, landingRectangle, cuttingPlane);
				/*
				foreach (var pin in newBlockPins.Pins)
				{
					Debug.Log(pin.CutPlanePosition);
				}
				*/
				//Debug.Log(cuttingPlane.PlanesCount);				
				if (landingPins != null && newBlockPins != null) 
				return (FitsConnectSystem.TryConnect(cuttingPlane, landingPins, newBlockPins, out pinsContainer)) ;		
            }
            pinsContainer = null;
			return false;
		}
		public bool TryGetLockZone(int cuttingPlaneId, out CuttingPlaneLockZone lockZone) => _lockZones.TryGetValue(cuttingPlaneId, out lockZone);
        public bool TryGetLockZone(CuttingPlanePosition coord, out CuttingPlaneLockZone lockZone)
		{
			if (_cuttingPlanes.TryGetValue(coord, out var cutPlane)) return _lockZones.TryGetValue(cutPlane.ID, out lockZone);
			else
			{
				lockZone = null;
				return false;
			}
		}

       	public ICuttingPlane GetCuttingPlane(int id) => _cuttingPlanesById[id];
		public ICuttingPlane GetOrCreateCutPlane(CuttingPlanePosition coord)
		{
			ICuttingPlane plane;
			if (!_cuttingPlanes.TryGetValue(coord, out plane))
			{
				plane = new CuttingPlanePlaceholder(_nextCuttingPlaneId++,coord);
				AddCuttingPlane(coord, plane);
			}
			return plane;
		}		
		
    }
}
