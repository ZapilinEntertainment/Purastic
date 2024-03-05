using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockStructureModule
	{	
		private readonly struct BlocksConnection
		{
			public readonly int ID;
			public readonly int BlockID_A, BlockID_B;
			public readonly FitInfo FitInfo;

			public BlocksConnection(int id, int blockID_A, int blockID_B, FitInfo fitInfo)
			{
				ID = id;
				BlockID_A= blockID_A;
				BlockID_B= blockID_B;
				FitInfo= fitInfo;
			}
		}		
		
		private int _nextBlockId = Utilities.GenerateInteger(), _nextConnectionID = Utilities.GenerateInteger();
		private readonly PlacedBlocksListHandler _blockList;		
		private Dictionary<int, PlacedBlock> _placedBlocks = new();
		private Dictionary<int, BlocksConnection> _connections = new();



        public BlockStructureModule(PlacedBlocksListHandler list)
		{
			_blockList= list;
		}

		public bool TryAddBlock(BlockProperties blockProperty, FitElementPosition fitPoint, FitInfo fitInfo, out PlacedBlock placedBlock)
		{
			if (_placedBlocks.TryGetValue(fitPoint.BlockId, out PlacedBlock block))
			{
				placedBlock = _blockList.RegisterBlock(blockProperty, fitPoint.WorldPosition);
				_placedBlocks.Add(placedBlock.ID, placedBlock);
				RegisterConnection(fitPoint.BlockId, placedBlock.ID, fitInfo);
				return true;
			}
			else
			{
				placedBlock = null;
				return false;
			}
		}

		private void RegisterConnection(int blockId_A, int blockId_B, FitInfo fitInfo)
		{
			int id = _nextConnectionID++;
            _connections.Add(id, new BlocksConnection(id, blockId_A, blockId_B, fitInfo));
        }
	}
}
