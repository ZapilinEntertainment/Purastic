using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class CuttingPlanesManager
	{
		// Imagine you cut the building at specific point (coordinate) and direction(face direction)
		private readonly IBlocksHost _blocksHost;
		private readonly PlacedBlocksListHandler _blocksList;
        private Dictionary<(BlockFaceDirection, float), ICuttingPlane> _cuttingPlanes = new();

		public CuttingPlanesManager(IBlocksHost blocksHost, PlacedBlocksListHandler blocksList)
		{
			_blocksList = blocksList;
            _blocksHost = blocksHost;

            _blocksHost.OnBlockPlacedEvent += OnBlockAdded;
		}

		private void OnBlockAdded(PlacedBlock block)
		{
			var planesContainer = block.Properties.GetPlanesList();
			var planes = planesContainer.Planes;
			Vector3 localPos = block.LocalPosition;
			foreach (var plane in planes)
			{
				AddFitPlane(plane);
			}
		}
		private void AddFitPlane(FitPlane plane)
		{
			BlockFaceDirection direction = plane.Direction;
			Vector3 normal = direction.Normal;
			float coordinate = GetCoordinate(plane.PlaneZeroPos, normal);
			ICuttingPlane cuttingPlane;

			var dataProvider = plane.GetDataProvider();
			var key = (direction, coordinate);
			if (!_cuttingPlanes.TryGetValue(key, out cuttingPlane))
			{
				cuttingPlane = new OneItemCuttingPlane(dataProvider, direction, coordinate);
				_cuttingPlanes.Add(key, cuttingPlane);
			}
			else _cuttingPlanes[key] = cuttingPlane.AddFitPlaneProvider(dataProvider);			
		}
        public bool TryGetFitElementPosition(Vector3 localPos, PlacedBlock block, out FitElementPosition fitPosition)
		{
            var direction = block.DefineFaceDirection(localPos);
            float coordinate = GetCoordinate(localPos, direction.Normal, out Vector3 projection);
			if (_cuttingPlanes.TryGetValue((direction, coordinate), out var cuttingPlane))
			{
				Vector2 planePos = direction.ToPlanePosition(projection);
				if (cuttingPlane.TryDefineFitPlane(planePos, out IFitPlanesDataProvider fitPlanes) && fitPlanes.TryGetPinIndex(planePos, out var pinIndex))
				{
					Vector2 pinPosition = fitPlanes.PinIndexToPosition(pinIndex);
					Vector3 pinLocalPos = cuttingPlane.GetLocalPos(pinPosition);
					fitPosition = new FitElementPosition(block.ID, direction, pinIndex, _blocksHost.ModelsHost.TransformPoint(pinLocalPos));
					return true;
				}
			}
			fitPosition = default;
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
    }
}
