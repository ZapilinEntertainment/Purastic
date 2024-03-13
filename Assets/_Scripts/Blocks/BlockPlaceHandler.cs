using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

    public struct BlocksCastResult
    {
        public readonly bool CanBePlaced;
        public readonly int BlockID;
        public readonly int BlockColliderID;
        public readonly VirtualPoint Point;
        public readonly FitElementStructureAddress StructureAddress;

        public BlocksCastResult(RaycastHit hit)
        {
            BlockID = -1;
            BlockColliderID = hit.colliderInstanceID;
            Point = new(hit.point, Quaternion.identity);
            CanBePlaced = false;
            StructureAddress = new();
        }
        public BlocksCastResult(FoundedFitElementPosition position)
        {
            StructureAddress = position.StructureAddress;
            CanBePlaced = true;
            BlockID = StructureAddress.BlockID;
            BlockColliderID = StructureAddress.BlockID;
            Point = position.WorldPoint;
        }
    }
    public class BlockPlaceHandler
	{        
        public bool IsPlacingAllowed { get; private set; } = false;
        private int? _lastBlocksHostId = null;
        private BlocksCastResult _selectedPoint;
        private IBlocksHost _selectedBlocksHost = null;
        private readonly ColliderListSystem _collidersList;
        public VirtualPoint ModelPoint => _selectedPoint.Point;
        public System.Action<bool> OnPlacingPermitChangedEvent;

        public BlockPlaceHandler(ColliderListSystem colliderListSystem)
        {
            _collidersList = colliderListSystem;
        }

        public void OnPinplaneHit(BlocksCastResult result)
        {            
            _selectedPoint = result;
            int hostID = result.BlockID;
            if (hostID != _lastBlocksHostId)
            {
                _lastBlocksHostId = hostID;
                 _collidersList.TryDefineBlockhost(_lastBlocksHostId.Value, out _selectedBlocksHost);
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
