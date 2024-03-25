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

		// some cutting planes are mirror to each other
		// due to no way of defining what normal is "primal" (up or down) on custom planes

		// also every face have its own 2d orths - because it normal can look up (y > 0) or down (y < 0), or even 0
		// ex: UP plane orths is (1,0)x(0,-1) when DOWN plane orths is (1,0)x(0,1)

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
			//Debug.Log(coordinate);
			_cuttingPlanes.Add(coordinate, plane);
			_cuttingPlanesById.Add(plane.ID, plane);
		}
		private void OnCuttingPlaneChanged(CuttingPlanePosition key, ICuttingPlane newPlane)
		{
            _cuttingPlanes[key] = newPlane;
			_cuttingPlanesById[newPlane.ID] = newPlane;
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
			BlockFaceDirection face = block.BlockFaceToLocalFace(fitPlaneConfig.Face);
			Vector3 normal = face.Normal;
			
			ICuttingPlane cuttingPlane;
			var dataProvider = fitPlaneConfig.CreateDataProvider(block.ID,subPlaneId,block, face);
            float coordinate = Utilities.DefineCutPlaneCoordinate(block.GetFaceZeroPointInLocalSpace(fitPlaneConfig.Face), normal);
            var key = new CuttingPlanePosition(face, coordinate);
			//Debug.Log($"{face}:{block.GetFaceZeroPointInLocalSpace(fitPlaneConfig.Face)}:{key}");
			//if (face.Direction == FaceDirection.Forward || face.Direction == FaceDirection.Back) Debug.Log(key);

            if (!_cuttingPlanes.TryGetValue(key, out cuttingPlane))
			{
				cuttingPlane = new OneItemCuttingPlane(_nextCuttingPlaneId++, dataProvider, face, coordinate);
				AddCuttingPlane(key, cuttingPlane);
			}
			else
			{
				OnCuttingPlaneChanged(key, cuttingPlane.AddFitPlaneProvider(dataProvider));
			}

			//Debug.Log(coordinate);
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
            //if (face.Direction == FaceDirection.Forward || face.Direction == FaceDirection.Back) Debug.Log(coordinate);
			if (_cuttingPlanes.TryGetValue(new(face, coordinate), out var cuttingPlane))
			{				
				Vector2 cutPlanePos = face.InverseVector(localPos);
				//if (face.Direction != FaceDirection.Up) Debug.Log($"{face}:{cutPlanePos}");
				//if (face.Direction != FaceDirection.Up) Debug.Log(cuttingPlane.TryDefineFitPlane(cutPlanePos, out IFitPlaneDataProvider ftp));
                if (cuttingPlane.TryDefineFitPlane(cutPlanePos, out IFitPlaneDataProvider fitPlane) && fitPlane.TryGetPinPosition(cutPlanePos, out var planeAddress))
				{
					var address = new FitElementStructureAddress(block.ID, cuttingPlane.ID, face, planeAddress);
					//Debug.Log(address.PlaneAddress);
					var facePoint = fitPlane.IndexToVirtualPoint(address.PlaneAddress.PinIndex);
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
				else
				{
					fitPosition = new FoundedFitElementPosition(
						BlocksHost.ID,
						new FitElementStructureAddress(block.ID, cuttingPlane.ID, face, default),
						default,
						normal,
						false
						);
					return false;
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

				var mirrorPlane = GetOrCreateCutPlane(cuttingPlane.GetMirrorPosition());
				var mirrorFace = mirrorPlane.Face;
				var mirrorRect = Utilities.ProjectBlock(mirrorFace, planningBlock);
				var connectFace = planningBlock.LocalFaceToBlockFace(cuttingPlane.Face.Mirror());
				var newBlockPins = planningBlock.Properties.GetPlanesList().CreateLandingPinsList(-1,planningBlock, connectFace, mirrorRect, mirrorPlane);

				int landingPinsCount = landingPins?.Pins.Count ?? 0,
					newBlockPinsCount = newBlockPins?.Pins.Count ?? 0;
                //Debug.Log(landingPins.Pins.Count);
                //Debug.Log(newBlockPins.Pins.Count);
				//Debug.Log(cuttingPlane.PlanesCount);				
				if (landingPinsCount != 0 && newBlockPinsCount != 0) 
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
