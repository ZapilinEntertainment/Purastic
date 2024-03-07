using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class TestConstructor : MonoBehaviour
	{
		[SerializeField] private Baseplate _baseplate;

		private async void Start()
		{
            await Awaitable.WaitForSecondsAsync(0.5f);
			int rootId = _baseplate.RootBlockId;
			bool x =_baseplate.TryAddDetail(
				new FitElementStructureAddress(rootId, FaceDirection.Up, new FitElementPlaneAddress(1, 1)),
				new PlacingBlockInfo(
					BlockPresetsList.GetProperty(BlockPreset.StandartBrick_1x1, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender)),
					PlacedBlockRotation.NoRotation
				));
            _baseplate.TryAddDetail(
                new FitElementStructureAddress(rootId, FaceDirection.Up, new FitElementPlaneAddress(4, 8)),
                new PlacingBlockInfo(
                    BlockPresetsList.GetProperty(BlockPreset.StandartBrick_2x4, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender)),
                    PlacedBlockRotation.NoRotation
                ));
            _baseplate.TryAddDetail(
                new FitElementStructureAddress(rootId, FaceDirection.Up, new FitElementPlaneAddress(8, 4)),
                new PlacingBlockInfo(
                    BlockPresetsList.GetProperty(BlockPreset.StandartBrick_2x4, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender)),
                    PlacedBlockRotation.NoRotation
                ));
        }
	}
}
