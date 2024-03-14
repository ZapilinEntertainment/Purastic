using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

    public enum BlockPositionStatus : byte { Undefined,CannotBePlaced, Obstructed, CanBePlaced}
    public struct BlocksCastResult
    {
        public readonly BlockPositionStatus Status;
        public readonly int BlockID;
        public readonly int BlockColliderID;
        public readonly VirtualPoint Point;
        public readonly FitElementStructureAddress StructureAddress;

        public BlocksCastResult(RaycastHit hit)
        {
            BlockID = -1;
            BlockColliderID = hit.colliderInstanceID;
            Point = new(hit.point, Quaternion.identity);
            Status = BlockPositionStatus.CannotBePlaced;
            StructureAddress = new();
        }
        public BlocksCastResult(FoundedFitElementPosition position)
        {
            StructureAddress = position.StructureAddress;
            Status = position.PositionIsObstructed ? BlockPositionStatus.Obstructed:BlockPositionStatus.CanBePlaced;
            BlockID = StructureAddress.BlockID;
            BlockColliderID = StructureAddress.BlockID;
            Point = position.WorldPoint;
        }
    }
    public class BlockPlaceHandler
	{        
        public BlockPositionStatus PositionStatus { get; private set; }
        private int? _lastBlocksHostId = null;
        private BlocksCastResult _selectedPoint;
        private IBlocksHost _selectedBlocksHost = null;
        private readonly ColliderListSystem _collidersList;
        public VirtualPoint ModelPoint => _selectedPoint.Point;
        public System.Action<BlockPositionStatus> OnPlacingPermitChangedEvent;

        public BlockPlaceHandler(ColliderListSystem colliderListSystem)
        {
            _collidersList = colliderListSystem;
        }

        public void OnPinplaneHit(BlocksCastResult result)
        {            
            //need to be reworked - it is executing every fixed frame, shall add dirty flags

            _selectedPoint = result;
            int hostID = result.BlockID;
            if (hostID != _lastBlocksHostId)
            {
                _lastBlocksHostId = hostID;
                 _collidersList.TryDefineBlockhost(_lastBlocksHostId.Value, out _selectedBlocksHost);
            }

            if (_selectedPoint.Status != PositionStatus)
            {
                PositionStatus = _selectedPoint.Status;
                OnPlacingPermitChangedEvent?.Invoke(PositionStatus);
            }
        }
        public bool TryAddDetail(PlacingBlockInfo placingBlockInfo) => _selectedBlocksHost?.TryAddDetail(_selectedPoint.StructureAddress, placingBlockInfo) ?? false;
    }
}
