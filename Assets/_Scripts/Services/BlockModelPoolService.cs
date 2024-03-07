using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class BlockModelPoolService
	{
		private class PooledModelsManager
		{
			private Queue<int> _pooledModelsIndices;
			private readonly BlockModelPoolService _poolService;
			public readonly BlockProperties Property;
			public int Count { get; private set; }
			
			public PooledModelsManager(BlockProperties property, BlockModelPoolService poolService)
			{
				Property = property;
				_poolService = poolService;
				Count = 0;
			}
			public void CacheModel(BlockModel model)
			{
				int id = _poolService.AddModelToPool(model);
				if (_pooledModelsIndices == null) _pooledModelsIndices = new();
				_pooledModelsIndices.Enqueue(id);
				Count++;
			}
			public bool TryGetModel(out BlockModel model)
			{
				if (Count == 0)
				{
					model = null;
					return false;
				}
				else
				{
					int index = _pooledModelsIndices.Dequeue();
					Count = _pooledModelsIndices.Count;
					if (Count == 0) _pooledModelsIndices = null;
					return _poolService.TryExtractModel(index, out model);
				}
			}
		}

		private int _nextModelID = Utilities.GenerateInteger();
		private const int MAX_COUNT = 10;
		private Dictionary<BlockProperties, PooledModelsManager> _managers = new();
		private Dictionary<int, BlockModel> _models = new();

		private int AddModelToPool(BlockModel model)
		{
			int id = _nextModelID++;
			_models.Add(id, model);
			model.OnReturnedToPool();
			return id;
		}
		private bool TryExtractModel(int id, out BlockModel model)
		{
			model = _models[id];
			_models.Remove(id);
			if (model != null)
			{
				model.OnSpawnedFromPool();
				return true;
			}
			else return false;
		}
		public void CacheModel(IPoolableModel model)
		{
			var blockModel = model as BlockModel;
			if (blockModel != null )
			{
				var property = blockModel.GetBlockProperty();
				PooledModelsManager manager;
				if (_managers.TryGetValue(property, out manager)) {
					if (manager.Count >= MAX_COUNT) goto POOLING_FAILED;
				}
				else
				{
					manager = new PooledModelsManager(property, this);
                    _managers.Add(property, manager);
				}
				manager.CacheModel(blockModel);
				return;
			}

			POOLING_FAILED:
			model.Dispose();
		}
		public bool TryGetCachedPart(BlockProperties block, out BlockModel model)
		{
			if (_managers.TryGetValue(block, out var manager))
			{
				return manager.TryGetModel(out model);
			}
			else
			{
				model = null;
				return false;
			}
		}
	}
}
