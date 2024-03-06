using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockPlaceHandler
	{
        private struct CastResult
        {
            public readonly bool CanBePlaced;
            public readonly int BlockColliderID;
            public readonly Vector3 HitPoint;
            public readonly FitElementStructureAddress FitPosition;

            public CastResult(RaycastHit hit)
            {
                BlockColliderID = hit.colliderInstanceID;
                HitPoint = hit.point;
                CanBePlaced = false;
                FitPosition = new();
            }
            public CastResult(FitElementStructureAddress position)
            {
                FitPosition= position;
                CanBePlaced = true;
                BlockColliderID = FitPosition.BlockID;
                HitPoint = FitPosition.WorldPosition;
            }
        }

        private int _lastBlocksHostId = -1;
        private CastResult _selectedPoint;
        private IBlocksHost _selectedBlocksHost = null;
        private readonly ColliderListSystem _collidersList;
        public Vector3 ModelPosition => _selectedPoint.HitPoint; 

        public BlockPlaceHandler(ColliderListSystem colliderListSystem)
        {
            _collidersList = colliderListSystem;
        }

        public void OnPinplaneHit(RaycastHit hit)
        {
            int hostID = hit.colliderInstanceID;
            _selectedPoint = new CastResult(hit);
            if (hostID != _lastBlocksHostId)
            {
                _lastBlocksHostId = hostID;
                _collidersList.TryDefineBlockhost(_lastBlocksHostId, out _selectedBlocksHost);
            }
            if (_selectedBlocksHost != null && _selectedBlocksHost.TryGetFitElementPosition(_selectedPoint.BlockColliderID, _selectedPoint.HitPoint, out var fitPosition)) 
                _selectedPoint = new(fitPosition);
        }
        public bool TryPinDetail(BlockProperties block) => _selectedBlocksHost?.TryPinDetail(_selectedPoint.FitPosition, block) ?? false;
    }
}
