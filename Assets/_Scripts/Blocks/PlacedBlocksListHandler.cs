using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public class PlacedBlocksListHandler
	{
		private int _rootBlockId;
		private int _nextID = Utilities.GenerateInteger();
		private Dictionary<int, PlacedBlock> _placedBlocks = new();

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

        public PlacedBlocksListHandler(BlockProperties rootBlock) {
			var root = RegisterBlock(rootBlock, Vector3.zero);
			_rootBlockId = root.ID;
		}

		public bool TryGetBlock(int id, out PlacedBlock block) => _placedBlocks.TryGetValue(id, out block);
		public PlacedBlock RegisterBlock(BlockProperties properties, Vector3 localPos)
		{
			int id = _nextID++;
			var placedBlock = new PlacedBlock(id, localPos, properties);
            _placedBlocks.Add(id, placedBlock);
			return placedBlock;
		}
	}
}
