using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class ConstructionVisualizer : MonoBehaviour
	{
		private bool _needRedraw = false;
		private BlockCreateService BlockCreateService => _resolver.Item1;
		private BlockModelPoolService CacheService => _resolver.Item3;
        private IBlocksHost _host;
		private PlacedBlocksListHandler _blocksHandler;
		private ComplexResolver<BlockCreateService, GameResourcesPack, BlockModelPoolService> _resolver;
		private List<BlockModel> _models = new();
		

		public void Setup(IBlocksHost host, PlacedBlocksListHandler blocksHandler) { 
			_host= host;
            _blocksHandler = blocksHandler;

            _resolver = new(OnResolved);
			_resolver.CheckDependencies();

			_host.OnBlockPlacedEvent += OnBlockPlaced;
        }

		private void OnResolved()
		{
            FullRedrawAsync();			
        }
        private void Update()
        {
			if (_needRedraw) FullRedrawAsync();
        }

        private async void OnBlockPlaced(PlacedBlock block)
		{
            var model = await BlockCreateService.CreateBlockModel(block.Properties);
            model.transform.SetParent(_host.ModelsHost, false);
            _models.Add(model);
        }
		public async void FullRedrawAsync()
		{
			int count = _models.Count;
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					CacheService.CacheModel(_models[i]);
				}
				_models.Clear();
			}			
			var blockData = _host.GetBlocks();
			Transform host = _host.ModelsHost;
			foreach (var data in blockData)
			{
                var block = await BlockCreateService.CreateBlockModel(data);
                block.transform.SetParent(host, false);
				_models.Add(block);
            }
			
		}
	}
}
