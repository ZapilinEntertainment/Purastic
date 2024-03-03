using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IBlockPlacer
	{
        public void OnDetailPlaced();
        public IPlaceable GetPlaceableModel();
        
    }
}
