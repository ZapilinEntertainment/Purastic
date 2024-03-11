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
            public readonly VirtualPoint Point;
            public readonly FitElementStructureAddress StructureAddress;

            public CastResult(RaycastHit hit)
            {
                BlockColliderID = hit.colliderInstanceID;
                Point = new( hit.point, Quaternion.identity);
                CanBePlaced = false;
                StructureAddress = new();
            }
            public CastResult(FoundedFitElementPosition position)
            {
                StructureAddress= position.StructureAddress;
                CanBePlaced = true;
                BlockColliderID = StructureAddress.BlockID;
                Point = position.WorldPoint;
            }
        }

        public bool IsPlacingAllowed { get; private set; } = false;
        private int _lastBlocksHostId = -1;
        private CastResult _selectedPoint;
        private IBlocksHost _selectedBlocksHost = null;
        private readonly ColliderListSystem _collidersList;
        public VirtualPoint ModelPoint => _selectedPoint.Point;
        public System.Action<bool> OnPlacingPermitChangedEvent;

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
            if (_selectedBlocksHost != null && _selectedBlocksHost.TryGetFitElementPosition(hit, out var fitPosition))
            {
                _selectedPoint = new(fitPosition);
            }
            if (_selectedPoint.CanBePlaced != IsPlacingAllowed)
            {
                IsPlacingAllowed = _selectedPoint.CanBePlaced;
                OnPlacingPermitChangedEvent?.Invoke(IsPlacingAllowed);
            }
        }
        public bool TryAddDetail(PlacingBlockInfo placingBlockInfo) => _selectedBlocksHost?.TryAddDetail(_selectedPoint.StructureAddress, placingBlockInfo) ?? false;
    }
}
