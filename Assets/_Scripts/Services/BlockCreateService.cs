using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class BlockCreateService
	{
		private AwaitableCompletionSource _gameResourcesCompletion = null;
		private GameResourcesPack _gameResources;
		private BlockModelCacheService _blockModelCacheService;
		private Coroutine _waitForResourcesCoroutine;
		private Dictionary<int, GameObject> _modelsDepot = new();

		public BlockCreateService()
		{
			ServiceLocatorObject.GetWhenLinkReady<GameResourcesPack>(OnGameResourcesLoaded);
			ServiceLocatorObject.GetWhenLinkReady((BlockModelCacheService cacheService) => _blockModelCacheService = cacheService);
		}
		private void OnGameResourcesLoaded(GameResourcesPack pack)
		{
			_gameResources = pack;
			if (_gameResourcesCompletion != null)
			{
				_gameResourcesCompletion.SetResult();
				_gameResourcesCompletion = null;
			}
		}

		public async Awaitable<BlockModel> CreateBlockModel(Block block)
		{
			int hashcode = block.GetHashCode();

			BlockModel blockModel;
			if (_blockModelCacheService?.TryGetCachedModel(block, out blockModel) ?? true)
			{
				GameObject modelLink;
				if (!_modelsDepot.TryGetValue(hashcode, out modelLink))
				{
					modelLink = await CreateModel(block);
					modelLink.SetActive(false);
					_modelsDepot.Add(hashcode, modelLink);
				}
				var model = Object.Instantiate(modelLink);
				blockModel = new GameObject("block" + hashcode.ToString()).AddComponent<BlockModel>();
                model.transform.SetParent(blockModel.transform, false);
                model.SetActive(true);
            }
			else
			{
				// todo
			}
            	
			blockModel.Setup(block);            
            return blockModel;
        }

		private async Awaitable<GameObject> CreateModel(Block block)
		{
			if (_gameResources == null)
			{
				if (_gameResourcesCompletion == null) _gameResourcesCompletion = new();
				await _gameResourcesCompletion.Awaitable;
			}
			Transform host = new GameObject("modelHost").transform;
			var model = GameObject.CreatePrimitive(PrimitiveType.Cube);
			model.transform.SetParent(host,false);
			var size = block.Size;
			model.transform.localPosition = 0.5f * size.y * Vector3.up;
			model.transform.localScale = size;
			var fitPlanes = block.FitPlanesHost.GetFitPlanes();
			if (fitPlanes.Count > 0)
			{
				foreach (var plane in fitPlanes)
				{
					Object.Instantiate(_gameResources.KnobPrefab, position: plane.Position, rotation: Quaternion.identity, parent: host);
				}
			}
			return host.gameObject;
		}
	}
}
