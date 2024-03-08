using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IPlaceable
	{
		public bool IsVisible { get; set; }
		public void SetPoint(Vector3 pos, Quaternion rotation);
		public void SetDrawMaterial(Material material);
		public BlockProperties GetBlockProperty();

		public void Dispose();
	}
}
