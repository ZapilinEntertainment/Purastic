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

		private BlockProperties(BlockMaterial material, Vector3 modelSize, int planesHash, int thick)
		{
			Material = material;
			ModelSize = modelSize;
			FitPlanesHash = planesHash;
			Thick= thick;
		}

        //platform :
        public  BlockProperties(FitsGridConfig config, BlockMaterial material, int thick)
		{
			Material = material;
			ModelSize = new Vector3(config.Width * GameConstants.BLOCK_SIZE, GameConstants.GetHeight(thick), config.Length * GameConstants.BLOCK_SIZE) ;
            FitPlanesHash = FitPlanesConfigsDepot.SaveConfig(
				new FitPlanesConfigList(
				new FitPlaneConfig(config, config.FitType, new BlockFaceDirection(FaceDirection.Up), new Vector3(-0.5f * config.Width * GameConstants.BLOCK_SIZE, 0.5f * GameConstants.GetHeight(thick), 0.5f * config.Width * GameConstants.BLOCK_SIZE))
				));
			Thick= thick;
		}
        public BlockProperties(FitPlaneConfig[] configs, BlockMaterial material, Vector3Int size)
        {
            Material = material;
            ModelSize = new Vector3(size.x * GameConstants.BLOCK_SIZE, GameConstants.GetHeight(size.y), size.z * GameConstants.BLOCK_SIZE);

            FitPlanesHash = FitPlanesConfigsDepot.SaveConfig(
                new FitPlanesConfigList(configs));
            Thick = size.y;
        }

        public override int GetHashCode()
        {
			return HashCode.Combine(Material.GetHashCode(), ModelSize.GetHashCode(), FitPlanesHash, Thick);
        }
		public BlockProperties ChangeMaterial(BlockMaterial material) => new (material, ModelSize, FitPlanesHash, Thick);


		public Vector2 GetProjectionSize(BlockFaceDirection face) { 
			var orths = new FaceOrths(face);
			return orths.InverseVector(ModelSize);
		}
		public FitPlanesConfigList GetPlanesList() => FitPlanesConfigsDepot.LoadConfig(FitPlanesHash);
		public IContactPlaneController CreateContactPlaneController(BlockFaceDirection face) => GetPlanesList().CreateContactPlaneController(face);
    }
}
