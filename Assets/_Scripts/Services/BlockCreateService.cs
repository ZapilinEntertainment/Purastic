using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class BlockCreateService
	{
		private int _pinplanesLayer = LayerConstants.GetDefinedLayer(DefinedLayer.Pinplane);
		private AwaitableCompletionSource _resolverCheck;
		private readonly ComplexResolver<GameResourcesPack, MaterialsDepot> _resolver;
		private GameResourcesPack GameResources => _resolver.Item1;
		private MaterialsDepot MaterialsDepot => _resolver.Item2;
		private BrickModelsPack _brickModelsPack;

		private BlockModelCacheService _blockModelCacheService;
		private Dictionary<int, GameObject> _modelsDepot = new();

		public BlockCreateService()
		{
            _resolverCheck = new AwaitableCompletionSource();
            _resolver = new(OnResolved);
			
			ServiceLocatorObject.GetWhenLinkReady((BlockModelCacheService cacheService) => _blockModelCacheService = cacheService);
			_resolver.CheckDependencies();
		}
		private void OnResolved()
		{
            _brickModelsPack = GameResources.BrickModelsPack;
            _resolverCheck.SetResult();			
		}

		public async Awaitable<BlockModel> CreateBlockModel(Block block)
		{
			
			int hashcode = block.GetHashCode();

			BlockModel blockModel;
			if (_blockModelCacheService == null || !_blockModelCacheService.TryGetCachedPart(block, out blockModel))
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

            if (!_resolver.AllDependenciesCompleted) await _resolverCheck.Awaitable;
            blockModel.Setup(block, MaterialsDepot.GetVisualMaterial(block.Material));            
            return blockModel;
        }

		private async Awaitable<GameObject> CreateModel(Block block)
		{
            if (!_resolver.AllDependenciesCompleted) await _resolverCheck.Awaitable;
            Transform host = new GameObject("modelHost").transform;
			var model = GameObject.Instantiate(_brickModelsPack.CubePrefab);		
			model.transform.SetParent(host,false);
			model.gameObject.layer = _pinplanesLayer;

			var size = block.Size;
			model.transform.localPosition = 0.5f * size.y * Vector3.up;
			model.transform.localScale = size;
			var fitPlanes = block.FitPlanesHost.GetFitPlanes();
			if (fitPlanes.Count > 0)
			{
				foreach (var plane in fitPlanes)
				{
					Object.Instantiate(_brickModelsPack.KnobPrefab, position: plane.Position, rotation: Quaternion.identity, parent: host);
				}
			}
			return host.gameObject;
		}
	}
}
