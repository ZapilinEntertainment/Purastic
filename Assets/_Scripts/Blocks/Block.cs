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
			FitPlanesHost = fitPlanes;
			Size = size;
			Material = material;
		}
		public Block(KnobGrid knobGrid, BlockMaterial material)
		{
			Material = material;
			Size = new Vector3(knobGrid.Width * GameConstants.BLOCK_SIZE, GameConstants.GetHeight(knobGrid.HeightInPlates), knobGrid.Length * GameConstants.BLOCK_SIZE) ;
			FitPlanesHost = knobGrid;
		}
        public override int GetHashCode()
        {
			return HashCode.Combine(Material.GetHashCode(), Size.GetHashCode(), FitPlanesHost.GetHashCode());
        }
    }
}
