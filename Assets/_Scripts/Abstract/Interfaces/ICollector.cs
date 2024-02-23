using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {
	public interface ICollector
	{
		//public void OnStartCollect(CollectZone zone);
		//public void OnStopCollect(CollectZone zone);
		//public void CollectItems(IList<VirtualCollectable> items, out BitArray result);
		public bool TryCollect(ICollectable collectable);
		//public bool TryCollect(VirtualCollectable collectable);
    }
}
