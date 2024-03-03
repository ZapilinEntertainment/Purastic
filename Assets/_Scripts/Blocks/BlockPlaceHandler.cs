using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockPlaceHandler
	{
        private int _lastBlocksHostId = -1;
        private PinPosition _selectedPosition;
        private IBlocksHost _selectedBlocksHost = null;
        private readonly ColliderListSystem _collidersList;
        public Vector3 ModelPosition => _selectedPosition.ModelPosition; 

        public BlockPlaceHandler(ColliderListSystem colliderListSystem)
        {
            _collidersList = colliderListSystem;
        }

        public void OnPinplaneHit(RaycastHit hit)
        {
            int hostID = hit.colliderInstanceID;
            _selectedPosition = new PinPosition(hit);
            if (hostID != _lastBlocksHostId)
            {
                _lastBlocksHostId = hostID;
                _collidersList.TryDefineBlockhost(_lastBlocksHostId, out _selectedBlocksHost);
            }
            if (_selectedBlocksHost != null) _selectedPosition = _selectedBlocksHost.PointToPin(_selectedPosition.ModelPosition);
        }
        public bool TryPinDetail(Block block) => _selectedBlocksHost?.TryPinDetail(_selectedPosition, block) ?? false;
    }
}
