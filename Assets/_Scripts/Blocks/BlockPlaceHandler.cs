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
            public readonly FitPosition FitPosition;

            public CastResult(RaycastHit hit)
            {
                BlockColliderID = hit.colliderInstanceID;
                HitPoint = hit.point;
                CanBePlaced = false;
                FitPosition = new();
            }
            public CastResult(FitPosition position)
            {
                FitPosition= position;
                CanBePlaced = true;
                BlockColliderID = FitPosition.BlockId;
                HitPoint = FitPosition.ModelPosition;
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
            if (_selectedBlocksHost != null) _selectedPoint = new( _selectedBlocksHost.PointToPin(_selectedPoint.BlockColliderID, _selectedPoint.HitPoint));
        }
        public bool TryPinDetail(BlockProperties block) => _selectedBlocksHost?.TryPinDetail(_selectedPoint.FitPosition, block) ?? false;
    }
}
