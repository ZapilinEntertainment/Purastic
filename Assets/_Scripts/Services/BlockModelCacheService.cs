using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class BlockModelCacheService
	{
		// INDEV

		public void CacheModel(ICachableModel blockModel)
		{
			GameObject.Destroy(blockModel.ModelObject);
		}
		public bool TryGetCachedPart(Block block, out BlockModel model)
		{
			model = null;
			return false;
		}
	}
}
