using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public class PlacedBlocksListHandler : SubcontainerModule
	{
		
		private int _nextID = Utilities.GenerateInteger();
		private Dictionary<int, PlacedBlock> _placedBlocks = new();
        public int RootBlockId { get; private set; }
		public PlacedBlock RootBlock => _placedBlocks[RootBlockId];
        public IReadOnlyCollection<BlockProperties> GetBlocksProperties()
		{
			var values = _placedBlocks.Values;
			int count = values.Count;
			var blockProperties = new BlockProperties[count];
			int i = 0;
			foreach (var value in values)
			{
				blockProperties[i] = value.Properties;
			}
			return blockProperties;
		}
        public IReadOnlyCollection<PlacedBlock> GetPlacedBlocks() => _placedBlocks.Values;

        public PlacedBlocksListHandler(PlacingBlockInfo initialBlock, Container container) : base(container) {
			var root = RegisterBlock(initialBlock, Vector3.zero);
			RootBlockId = root.ID;
		}

		public bool TryGetBlock(int id, out PlacedBlock block) => _placedBlocks.TryGetValue(id, out block);
		public PlacedBlock RegisterBlock(PlacingBlockInfo placingBlockInfo, Vector3 localPos)
		{
			int id = _nextID++;
			var placedBlock = new PlacedBlock(id, localPos, placingBlockInfo);
            _placedBlocks.Add(id, placedBlock);
			return placedBlock;
		}
        public PlacedBlock RegisterBlock(VirtualBlock block)
        {
            int id = _nextID++;
            var placedBlock = new PlacedBlock(id, block);
            _placedBlocks.Add(id, placedBlock);
            return placedBlock;
        }
    }
}
