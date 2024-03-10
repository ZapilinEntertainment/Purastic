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
            if (!(_baseplate as ILateInitializable).IsInitialized)
            {
                var aws = new AwaitableCompletionSource();
                _baseplate.InitStatusModule.OnInitializedEvent += aws.SetResult;
                await aws.Awaitable;
                aws = null;
            }
			int rootId = _baseplate.RootBlockId;
			bool x =_baseplate.TryAddDetail(
				_baseplate.FormPlateAddress(new Vector2Byte(2,2)),
				new PlacingBlockInfo(
					BlockPresetsDepot.GetProperty(BlockPreset.StandartBrick_1x1, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender))
                ));
            
            _baseplate.TryAddDetail(
                _baseplate.FormPlateAddress(new Vector2Byte(4, 8)),
                new PlacingBlockInfo(
                    BlockPresetsDepot.GetProperty(BlockPreset.StandartBrick_2x4, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender))
                ));
            _baseplate.TryAddDetail(
                _baseplate.FormPlateAddress(new Vector2Byte(8, 4)),
                new PlacingBlockInfo(
                    BlockPresetsDepot.GetProperty(BlockPreset.StandartBrick_2x4, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender))
                ));
        }
	}
}
