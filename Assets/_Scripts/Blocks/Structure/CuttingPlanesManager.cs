using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	
	public class CuttingPlanesManager : SubcontainerModule
	{
		// Imagine you cut the building at specific point (coordinate) and direction(face direction)
		private int _nextCuttingPlaneId = 1;
        private Dictionary<CuttingPlaneCoordinate, ICuttingPlane> _cuttingPlanes = new();
		private Dictionary<int, CuttingPlaneLockZone> _lockZones = new();
		private Dictionary<int, ICuttingPlane> _cuttingPlanesById = new();
		private readonly ComplexResolver<IBlocksHost, PlacedBlocksListHandler, BlockStructureModule> _localResolver;
		protected IBlocksHost BlocksHost => _localResolver.Item1;
		protected PlacedBlocksListHandler BlocksList => _localResolver.Item2;
		protected BlockStructureModule BlocksStructure => _localResolver.Item3;

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
		private void AddCuttingPlane(CuttingPlaneCoordinate coordinate, ICuttingPlane plane)
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
			AddLockPoints(lockedPins.CutPlaneA_id, lockedPins.GetLockedPinsA());
			AddLockPoints(lockedPins.CutPlaneB_id, lockedPins.GetLockedPinsB());
            

			void AddLockPoints(int id, IReadOnlyCollection<FitElementPlaneAddress> points)
			{
                CuttingPlaneLockZone lockZone;
                if (!_lockZones.TryGetValue(id, out lockZone))
                {
                    lockZone = new CuttingPlaneLockZone(id, points);
                    _lockZones.Add(id, lockZone);
                }
                else lockZone.AddLockedPins(points);
            }
        }
		private void AddFitPlane(byte subPlaneId, FitPlaneConfig fitPlaneConfig, PlacedBlock block)
		{
			BlockFaceDirection face = fitPlaneConfig.Face;
			Vector3 normal = face.Normal;
			Vector3 zeroPosInModelSpace = block.FacePositionToModelPosition(fitPlaneConfig.FaceZeroPos, face);

			float coordinate = GetCoordinate(zeroPosInModelSpace, normal);
			ICuttingPlane cuttingPlane;

			var dataProvider = fitPlaneConfig.CreateDataProvider(subPlaneId,block, face);
			var key = new CuttingPlaneCoordinate(face, coordinate);

			if (!_cuttingPlanes.TryGetValue(key, out cuttingPlane))
			{
				cuttingPlane = new OneItemCuttingPlane(_nextCuttingPlaneId++, dataProvider, face, coordinate);
				AddCuttingPlane(key, cuttingPlane);
			}
			else _cuttingPlanes[key] = cuttingPlane.AddFitPlaneProvider(dataProvider);
			//Debug.Log(key);
		}
		public void AddLockZones(int cutPlaneID, List<FitElementPlaneAddress> lockedPins)
		{
			CuttingPlaneLockZone zone;
            if (!_lockZones.TryGetValue(cutPlaneID, out zone))
            {
                zone = new CuttingPlaneLockZone(cutPlaneID);
                _lockZones.Add(cutPlaneID, zone);
            }
			zone.AddLockedPins(lockedPins);
        }

		public bool TryGetCuttingPlane(FitElementStructureAddress address, out ICuttingPlane plane) => _cuttingPlanesById.TryGetValue(address.CutPlaneID, out plane);
        public bool TryGetCuttingPlane(PlacedBlock block, BlockFaceDirection localDirection, out ICuttingPlane plane)
        {
            var coords = GetCutPlaneCoordinate(block, localDirection);
			//Debug.Log(coords);
            return _cuttingPlanes.TryGetValue(coords, out plane);
        }
        public bool TryGetFitElementPosition(Vector3 localPos,Vector3 normal, PlacedBlock block, out FoundedFitElementPosition fitPosition)
		{
            var face = new BlockFaceDirection(normal);
            float coordinate = GetCoordinate(localPos, face.Normal);
			if (_cuttingPlanes.TryGetValue(new(face, coordinate), out var cuttingPlane))
			{
				Vector2 cutPlanePos = face.LocalToFaceDirection(localPos);
				//Debug.Log($"{localPos} -> {planePos}");
				if (cuttingPlane.TryDefineFitPlane(cutPlanePos, out IFitPlaneDataProvider fitPlane) && fitPlane.TryGetPinPosition(cutPlanePos, out var planeAddress))
				{
					var address = new FitElementStructureAddress(block.ID, cuttingPlane.ID, face, planeAddress);
					
					var facePoint = fitPlane.GetFitElementFaceVirtualPoint(address.PlaneAddress.PinIndex);
					var localPoint = new VirtualPoint(cuttingPlane.CutPlaneToLocalPos(facePoint.Position), cuttingPlane.Face.ToRotation() * facePoint.Rotation);
					var worldPoint = new VirtualPoint(BlocksHost.TransformPosition(localPoint.Position), BlocksHost.ModelsHost.rotation * localPoint.Rotation);
                    fitPosition = new FoundedFitElementPosition(BlocksHost.ID, address, worldPoint, normal);
                    return true;
				}
			}
			//else Debug.Log(coordinate);
			fitPosition = default;
			return false;
        }
		public bool TryConnectNewBlock(PlacedBlock blockBase, FitElementStructureAddress address, VirtualBlock planningBlock, out ConnectedAndLockedPinsContainer pinsContainer)
		{
			var key = GetCutPlaneCoordinate(blockBase,address.ContactFace);
			if (_cuttingPlanes.TryGetValue(key, out var cuttingPlane) )
			{
				var landingRectangle = Utilities.ProjectBlock(cuttingPlane.Face, planningBlock);
				//Debug.Log(landingRectangle);

                var landingPins = cuttingPlane.GetLandingPinsList(landingRectangle);
				var connectFace = planningBlock.Rotation.InverseDirection(cuttingPlane.Face.Inverse());
				var newBlockPins = planningBlock.Properties.GetPlanesList().CreateLandingPinsList(planningBlock, connectFace, landingRectangle, cuttingPlane);
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

        private float GetCoordinate(Vector3 localPos, Vector3 planeNormal)
		{
            Vector3 projectedPos = Vector3.ProjectOnPlane(localPos, planeNormal);
            return Vector3.Dot(localPos - projectedPos, planeNormal);
        }
		public CuttingPlaneCoordinate GetCutPlaneCoordinate(PlacedBlock block, BlockFaceDirection face) => new CuttingPlaneCoordinate(face, GetCoordinate(block.GetFaceZeroPointInLocalSpace(face), face.Normal));
		public ICuttingPlane GetOrCreateCutPlane(CuttingPlaneCoordinate coord)
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
