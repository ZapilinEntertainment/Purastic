using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	
	public class CuttingPlanesManager
	{
		// Imagine you cut the building at specific point (coordinate) and direction(face direction)
		private int _nextCuttingPlaneId = 1;
		private readonly IBlocksHost _blocksHost;
		private readonly PlacedBlocksListHandler _blocksList;
        private Dictionary<CuttingPlaneCoordinate, ICuttingPlane> _cuttingPlanes = new();
		private Dictionary<int, CuttingPlaneLockZone> _lockZones = new();

		public CuttingPlanesManager(IBlocksHost blocksHost, PlacedBlocksListHandler blocksList, BlockStructureModule structureModule)
		{
			_blocksList = blocksList;
            _blocksHost = blocksHost;

            var blocks = _blocksList.GetPlacedBlocks();
            if (blocks.Count > 0) foreach (var block in blocks) OnBlockAdded(block);
            _blocksHost.OnBlockPlacedEvent += OnBlockAdded;

			var connections = structureModule.GetConnections();
			if (connections.Count > 0)
			{
				foreach (var connection in connections) OnConnectionCreated(connection);
			}
			structureModule.OnConnectionCreatedEvent += OnConnectionCreated;
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
			BlockFaceDirection direction = plane.Face;
			Vector3 normal = direction.Normal;
			float coordinate = GetCoordinate(plane.PlaneZeroPos, normal);
			ICuttingPlane cuttingPlane;

			var dataProvider = plane.CreateDataProvider(block);
			var key = new CuttingPlaneCoordinate(direction, coordinate);
			if (!_cuttingPlanes.TryGetValue(key, out cuttingPlane))
			{
				cuttingPlane = new OneItemCuttingPlane(_nextCuttingPlaneId++, dataProvider, direction, coordinate);
				_cuttingPlanes.Add(key, cuttingPlane);
			}
			else _cuttingPlanes[key] = cuttingPlane.AddFitPlaneProvider(dataProvider);			
		}
		public void AddLockZones(List<LockedPin> lockedPins)
		{
            CuttingPlaneLockZone cachedLockZone = null;
			int cachedPlaneId = -1;
			foreach (var pin in lockedPins)
			{
				if (pin.CuttingPlaneID == cachedPlaneId) cachedLockZone.AddLockedPin(pin.PlaneAddress);
				else
				{
					cachedPlaneId = pin.CuttingPlaneID;
					if (!_lockZones.TryGetValue(cachedPlaneId, out cachedLockZone))
					{
						cachedLockZone = new CuttingPlaneLockZone(cachedPlaneId);
						_lockZones.Add(cachedPlaneId, cachedLockZone);
					}
                    cachedLockZone.AddLockedPin(pin.PlaneAddress);
                }
			}
		}

        public bool TryGetFitElementPosition(Vector3 localPos, PlacedBlock block, out FitElementStructureAddress fitPosition)
		{
            var direction = block.DefineFaceDirection(localPos);
            float coordinate = GetCoordinate(localPos, direction.Normal, out Vector3 projection);
			if (_cuttingPlanes.TryGetValue(new(direction, coordinate), out var cuttingPlane))
			{
				Vector2 planePos = direction.ToPlanePosition(projection);
				if (cuttingPlane.TryDefineFitPlane(planePos, out IFitPlanesDataProvider fitPlanes) && fitPlanes.TryGetPinPosition(planePos, out var pinIndex))
				{
					Vector2 pinPosition = fitPlanes.PinIndexToPosition(pinIndex);
					Vector3 pinLocalPos = cuttingPlane.GetLocalPos(pinPosition);
					fitPosition = new FitElementStructureAddress(block.ID, direction, pinIndex, _blocksHost.ModelsHost.TransformPoint(pinLocalPos));
					return true;
				}
			}
			fitPosition = default;
			return false;
        }
		public bool TryConnectNewBlock(PlacedBlock blockBase, FitElementStructureAddress address, FitPlanesConfigList newBlockPlanes, out List<LockedPin> connectedPins)
		{
			var key = BlockPlanePosition(blockBase,address.Direction);
			if (_cuttingPlanes.TryGetValue(key, out var cuttingPlane) )
			{
				cuttingPlane.(address.PlanePinPosition)
				var landingPins = cuttingPlane.GetLandingPinsList();
				var newBlockPins = newBlockPlanes.GetLandingPins(blockBase.Rotation.TransformDirection(address.Direction));

				return FitsConnectSystem.TryConnect(landingPins, newBlockPins, out connectedPins);				
            }
			connectedPins = null;
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
		private CuttingPlaneCoordinate BlockPlanePosition(PlacedBlock block, BlockFaceDirection direction) => new CuttingPlaneCoordinate(direction, GetCoordinate(block.LocalPosition, direction.Normal));
    }
}
