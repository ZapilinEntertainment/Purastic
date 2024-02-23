using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class BlockModelCacheService
	{
		// INDEV

		public void CacheModel(BlockModel blockModel)
		{
			GameObject.Destroy(blockModel.gameObject);
		}
		public bool TryGetCachedModel(Block block, out BlockModel model)
		{
			model = null;
			return false;
		}
	}
}
