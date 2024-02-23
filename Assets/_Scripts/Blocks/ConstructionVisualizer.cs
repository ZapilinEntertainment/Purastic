using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class ConstructionVisualizer
	{
		private BlockCreateService BlockCreateService => _resolver.Item1;
        private readonly Baseplate _baseplate;
		private readonly ComplexResolver<BlockCreateService, GameResourcesPack> _resolver;
		

		public ConstructionVisualizer(Baseplate platform) { 
			_baseplate = platform;
			_resolver = new(OnResolved);
			_resolver.CheckDependencies();
        }

		private void OnResolved()
		{
            RedrawAsync();			
        }

		public async void RedrawAsync()
		{
			var blockData = _baseplate.ToBlock();
			var block = await BlockCreateService.CreateBlockModel(blockData);
			block.transform.SetParent(_baseplate.transform, false);
		}
	}
}
