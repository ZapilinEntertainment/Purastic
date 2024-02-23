using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class Block
	{
		public readonly BlockMaterial Material;
		public readonly Vector3 Size;
		public readonly IFitPlanesContainer FitPlanesHost;

		public Block(IFitPlanesContainer fitPlanes, Vector3 size, BlockMaterial material)
		{
			this.FitPlanesHost = fitPlanes;
			this.Size = size;
			this.Material = material;
		}
        public override int GetHashCode()
        {
			return HashCode.Combine(Material.GetHashCode(), Size.GetHashCode(), FitPlanesHost.GetHashCode());
        }
    }
}
