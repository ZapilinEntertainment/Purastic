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

            var material = new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender);

            AddDetail(new Vector2Byte(1, 1), BlockPreset.StandartBrick_1x1);
            AddDetail(new Vector2Byte(4, 8), BlockPreset.StandartBrick_2x4);
            AddDetail(new Vector2Byte(8, 4), BlockPreset.StandartBrick_2x4);
            AddDetail(new Vector2Byte(7, 7), BlockPreset.StandartBrick_2x2);

            void AddDetail(Vector2Byte index, BlockPreset preset)
            {
                if (_baseplate.TryFormPlateAddress(index, out var structureAddress))
                {
                    _baseplate.TryAddDetail(structureAddress, new PlacingBlockInfo(
                        BlockPresetsDepot.GetProperty(preset, material)
                    ));
                }
            }

            /*
            if (_baseplate.CutPlanesDataProvider.TryGetLockZone(Utilities.DefineCutPlaneCoordinate(_baseplate.RootBlock, BlockFaceDirection.Up), out var zone)) {
                var points = zone.LockedElements;
                foreach (var point in points)
                {
                    Debug.Log(point);
                }
            }
            */
        }
	}
}
