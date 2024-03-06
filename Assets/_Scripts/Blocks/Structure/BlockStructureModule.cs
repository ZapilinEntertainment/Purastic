using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockStructureModule
	{	
		private int _nextConnectionID = Utilities.GenerateInteger();
		private readonly PlacedBlocksListHandler _blockList;		
		private Dictionary<int, PlacedBlock> _placedBlocks = new();
		private Dictionary<int, BlocksConnection> _connections = new();
		public System.Action<BlocksConnection> OnConnectionCreatedEvent;

		public IReadOnlyCollection<BlocksConnection> GetConnections() => _connections.Values;


        public BlockStructureModule(PlacedBlocksListHandler list)
		{
			_blockList= list;
		}

		public bool TryAddBlock(BlockProperties blockProperty,BlockFaceDirection placeDirection, FitElementStructureAddress fitPoint, List<LockedPin> connectedPins, out PlacedBlock placedBlock)
		{
			if (_placedBlocks.TryGetValue(fitPoint.BlockID, out PlacedBlock baseBlock))
			{
				placedBlock = _blockList.RegisterBlock(blockProperty, fitPoint.WorldPosition);
				_placedBlocks.Add(placedBlock.ID, placedBlock);
				RegisterConnection(baseBlock, placedBlock, fitPoint.Direction,placeDirection , connectedPins);
				return true;
			}
			else
			{
				placedBlock = null;
				return false;
			}
		}

		private void RegisterConnection(PlacedBlock blockA, PlacedBlock blockB, BlockFaceDirection directionA, BlockFaceDirection directionB, List<LockedPin> connectedPins)
		{
			int id = _nextConnectionID++;
			var connection = new BlocksConnection(id, blockA, blockB, directionA, directionB, connectedPins);
            _connections.Add(id, connection);
			OnConnectionCreatedEvent?.Invoke(connection);
        }
	}
}
