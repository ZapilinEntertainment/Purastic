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
		public readonly int Thick;
		public BlockProperties(Vector3 size, BlockMaterial material, int planesHash, int thick)
		{
			FitPlanesHash = planesHash;
			ModelSize = size;
			Material = material;
			Thick= thick;
		}
		public BlockProperties(FitsGridConfig grid, BlockMaterial material, int thick)
		{
			Material = material;
			ModelSize = new Vector3(grid.Width * GameConstants.BLOCK_SIZE, GameConstants.GetHeight(thick), grid.Length * GameConstants.BLOCK_SIZE) ;
            FitPlanesHash = grid.GetHashCode();
			Thick= thick;
		}
        public override int GetHashCode()
        {
			return HashCode.Combine(Material.GetHashCode(), ModelSize.GetHashCode(), FitPlanesHash);
        }
		public FitPlanesConfigList GetPlanesList() => FitPlanesConfigsDepot.LoadConfig(FitPlanesHash);
    }
}
