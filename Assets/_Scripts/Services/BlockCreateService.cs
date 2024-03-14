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

		private BlockModelPoolService _blockModelCacheService;

		public BlockCreateService()
		{
            _resolverCheck = new AwaitableCompletionSource();
            _resolver = new(OnResolved);
			
			ServiceLocatorObject.GetWhenLinkReady((BlockModelPoolService cacheService) => _blockModelCacheService = cacheService);
			_resolver.CheckDependencies();
		}
		private void OnResolved()
		{
            _brickModelsPack = GameResources.BrickModelsPack;
            _resolverCheck.SetResult();			
		}

		public async Awaitable<BlockModel> CreateBlockModel(BlockProperties block)
		{
			BlockModel blockModel;
			if (_blockModelCacheService == null || !_blockModelCacheService.TryGetCachedPart(block, out blockModel))
			{
                blockModel = await CreateModel(block);
            }

            if (!_resolver.AllDependenciesCompleted) await _resolverCheck.Awaitable;
            blockModel.Setup(block, MaterialsDepot.GetVisualMaterial(block.Material));            
            return blockModel;
        }

		private async Awaitable<BlockModel> CreateModel(BlockProperties properties)
		{
            if (!_resolver.AllDependenciesCompleted) await _resolverCheck.Awaitable;
			GameObject gameObject = new ("block" + properties.GetHashCode());
            Transform host = gameObject.transform;
			var model = GameObject.Instantiate(_brickModelsPack.CubePrefab);		
			model.transform.SetParent(host,false);
			model.gameObject.layer = _pinplanesLayer;

			var size = properties.ModelSize;
			model.transform.localPosition = Vector3.zero;//0.5f * size.y * Vector3.up;
			model.transform.localScale = size;

			VirtualBlock virtualBlock = new VirtualBlock(model.transform.position, new PlacingBlockInfo(properties));
			var fitPlanes = FitPlanesConfigsDepot.LoadConfig(properties.FitPlanesHash).Planes;
            // block always have at least 1 plane
            var registrationList = new Dictionary<FitElementPlaneAddress, GameObject>();
            for (byte i = 0; i < fitPlanes.Count; i++)
            {
                var plane = fitPlanes[i];
                var pinsPositions = plane.PinsConfiguration.GetAllPinsInPlaneSpace();
                var prefab = _brickModelsPack.GetFitElementPrefab(plane.FitType);
                var rotation = plane.Face.ToRotation();

                foreach (var pin in pinsPositions)
                {
                    var pinModel = Object.Instantiate(prefab, position: virtualBlock.FacePositionToModelPosition(pin.PlanePosition, plane.Face), rotation: rotation, parent: host);
                    registrationList.Add(new FitElementPlaneAddress(i, pin.Index), pinModel);
                }
            }

			var blockModel = gameObject.AddComponent<BlockModel>();
			blockModel.InitializeModel(registrationList);
			return blockModel;
		}
	}
}
