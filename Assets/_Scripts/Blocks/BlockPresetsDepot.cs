using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum BlockPreset : byte { Undefined, StandartBrick_1x1,StandartBrick_2x2, StandartBrick_2x4}
    public static class BlockPresetExtensions
    {
        public static FitElementPlaneAddress DefaultConnectPin(this BlockPreset preset) => new FitElementPlaneAddress(1, Vector2Byte.zero);
    }
	public static class BlockPresetsDepot
	{
		public static BlockProperties GetProperty(BlockPreset preset, BlockMaterial material)
		{
			switch (preset)
			{
                case BlockPreset.StandartBrick_1x1:
                    {
                        Vector3Int dimensions = new Vector3Int(1, 3, 1);
                        var configs = new FitPlaneConfig[2]
                       {
                            new FitPlaneConfig(FitType.Knob, dimensions, FaceDirection.Up),
                            new FitPlaneConfig(FitType.Slot, dimensions, FaceDirection.Down)
                       };
                        return new BlockProperties(configs, material, dimensions);
                    }
				case BlockPreset.StandartBrick_2x2:
					{
                        Vector3Int dimensions = new Vector3Int(2, 3, 2);
                        var configs = new FitPlaneConfig[2]
                       {
                            new FitPlaneConfig(FitType.Knob, dimensions, FaceDirection.Up),
                            new FitPlaneConfig(FitType.Slot, dimensions, FaceDirection.Down)
                       };
                        return new BlockProperties(configs, material, dimensions);
                    }
                case BlockPreset.StandartBrick_2x4:
					{
						Vector3Int dimensions = new Vector3Int(2, 3, 4);
                        var configs = new FitPlaneConfig[2]
					   {
							new FitPlaneConfig(FitType.Knob, dimensions, FaceDirection.Up),
							new FitPlaneConfig(FitType.Slot, dimensions, FaceDirection.Down)
					   };
                        return new BlockProperties(configs, material, dimensions);                       
					}
				default: return default;
			}
		}
	}
}
