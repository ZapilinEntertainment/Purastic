using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IPlaceable
	{
		public bool IsVisible { get; set; }
		public void SetPlacePosition(Vector3 pos);
		public void SetDrawMaterial(Material material);
		public Block GetBlockData();

		public void Dispose();
	}
}
