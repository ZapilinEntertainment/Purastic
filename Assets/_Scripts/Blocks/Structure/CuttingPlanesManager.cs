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
			foreach (var plane in planes)
			{
				AddFitPlane(plane, block);
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
		private void AddFitPlane(FitPlaneConfig plane, PlacedBlock block)
		{
			BlockFaceDirection face = plane.Face;
			Vector3 normal = face.Normal;
			float coordinate = GetCoordinate(plane.FaceZeroPos, normal);
			ICuttingPlane cuttingPlane;

			var dataProvider = plane.CreateDataProvider(block, face);
			var key = new CuttingPlaneCoordinate(face, coordinate);

			if (!_cuttingPlanes.TryGetValue(key, out cuttingPlane))
			{
				cuttingPlane = new OneItemCuttingPlane(_nextCuttingPlaneId++, dataProvider, face, coordinate);
				AddCuttingPlane(key, cuttingPlane);
			}
			else _cuttingPlanes[key] = cuttingPlane.AddFitPlaneProvider(dataProvider);
			//Debug.Log(key.ToString());
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
        public bool TryGetFitElementPosition(Vector3 localPos, PlacedBlock block, out FitElementStructureAddress fitPosition)
		{
            var direction = block.DefineFaceDirection(localPos);
            float coordinate = GetCoordinate(localPos, direction.Normal, out Vector3 projection);
			if (_cuttingPlanes.TryGetValue(new(direction, coordinate), out var cuttingPlane))
			{
				Vector2 planePos = direction.InverseVector(projection);
				if (cuttingPlane.TryDefineFitPlane(planePos, out IFitPlaneDataProvider fitPlanes) && fitPlanes.TryGetPinPosition(planePos, out var planeAddress))
				{
					fitPosition = new FitElementStructureAddress(block.ID,cuttingPlane.ID, direction, planeAddress);
					return true;
				}
			}
			Debug.Log("no cutting plane");
			fitPosition = default;
			return false;
        }
		public bool TryConnectNewBlock(PlacedBlock blockBase, FitElementStructureAddress address, VirtualBlock planningBlock, out ConnectedAndLockedPinsContainer pinsContainer)
		{
			var key = GetCutPlaneCoordinate(blockBase,address.ContactFace);
			if (_cuttingPlanes.TryGetValue(key, out var cuttingPlane) )
			{
				var landingRectangle = Utilities.ProjectBlock(cuttingPlane.Face, planningBlock);

                var landingPins = cuttingPlane.GetLandingPinsList(landingRectangle);
				var connectFace = planningBlock.Rotation.InverseDirection(cuttingPlane.Face.Inverse());
				var newBlockPins = planningBlock.Properties.GetPlanesList().CreateLandingPinsList(blockBase, connectFace, landingRectangle, cuttingPlane);

				if (landingPins != null && newBlockPins != null) 
				return (FitsConnectSystem.TryConnect(cuttingPlane, landingPins, newBlockPins, out pinsContainer)) ;		
            }
            pinsContainer = null;
			return false;
		}

        private float GetCoordinate(Vector3 localPos, Vector3 planeNormal)
		{
            Vector3 projectedPos = Vector3.Project(localPos, planeNormal);
            return Vector3.Dot(localPos - projectedPos, planeNormal);
        }
        private float GetCoordinate(Vector3 localPos, Vector3 planeNormal, out Vector3 projectionVector)
        {
            projectionVector = Vector3.Project(localPos, planeNormal);
            return Vector3.Dot(localPos - projectionVector, planeNormal);
        }
		public CuttingPlaneCoordinate GetCutPlaneCoordinate(PlacedBlock block, BlockFaceDirection direction) => new CuttingPlaneCoordinate(direction, GetCoordinate(block.LocalPosition, direction.Normal));
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
		public ICuttingPlane GetUpLookingPlane(byte height = 0) => GetOrCreateCutPlane(new CuttingPlaneCoordinate(new BlockFaceDirection(FaceDirection.Up), height));
    }
}
