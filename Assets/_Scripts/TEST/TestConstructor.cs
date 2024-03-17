using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

    
	public sealed class TestConstructor : MonoBehaviour
	{
        [System.Serializable]
        public struct ConstructionBlockInfo
        {
            public Vector2Int Position;
            public BlockPreset Preset;
            public Quaternion Rotation;

            public Vector2Byte PlatePosition => new Vector2Byte(Position);
        }

        [SerializeField] private Baseplate _baseplate;
        [SerializeField] private List<ConstructionBlockInfo> _blocks;

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

            foreach (var item in _blocks)
            {
                if (_baseplate.TryFormPlateAddress(item.PlatePosition, out var structureAddress))
                {
                    _baseplate.TryAddDetail(structureAddress, new PlacingBlockInfo(
                        item.Preset.DefaultConnectPin(),
                        BlockPresetsDepot.GetProperty(item.Preset, material),
                        GameConstants.DefaultPlacingFace,
                        item.Rotation
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
