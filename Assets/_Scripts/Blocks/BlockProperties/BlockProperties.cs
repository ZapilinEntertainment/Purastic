using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public struct BlockProperties
	{
		public readonly BlockMaterial Material;
		public readonly Vector3 ModelSize;
		public readonly int FitPlanesHash;
		public BlockProperties(Vector3 size, BlockMaterial material, int planesHash)
		{
			FitPlanesHash = planesHash;
			ModelSize = size;
			Material = material;
		}
		public BlockProperties(FitsGrid knobGrid, BlockMaterial material)
		{
			Material = material;
			ModelSize = new Vector3(knobGrid.Width * GameConstants.BLOCK_SIZE, GameConstants.GetHeight(knobGrid.HeightInPlates), knobGrid.Length * GameConstants.BLOCK_SIZE) ;
            FitPlanesHash = knobGrid.GetHashCode();
		}
        public override int GetHashCode()
        {
			return HashCode.Combine(Material.GetHashCode(), ModelSize.GetHashCode(), FitPlanesHash);
        }
		public FitPlanesList GetPlanesList() => FitPlanesConfigsDepot.LoadConfig(FitPlanesHash);
    }
}
