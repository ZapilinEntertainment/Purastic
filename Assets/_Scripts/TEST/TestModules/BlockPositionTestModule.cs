using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockPositionTestModule : MonoBehaviour
	{
        [SerializeField] private bool _redraw = false;
        [SerializeField] private Baseplate _baseplate;
		[SerializeField] private Vector2Int _fitPosition, _initalPin;
		[SerializeField] private BlockPreset _preset;
        [SerializeField] private BlockColor _color = BlockColor.Lavender;
		[SerializeField] private Quaternion _rotation;
        private Transform _modelTransform;
        private BlockProperties _blockProperties;
        private Vector3 _rotationAxlePoint;
        private byte _initialPinPlane => _preset.DefaultConnectPin().SubPlaneId;
        private FitElementPlaneAddress PlaneAddress => new FitElementPlaneAddress(_initialPinPlane, new Vector2Byte(_initalPin));
        private PlacingBlockInfo PlacingInfo => new (PlaneAddress, _blockProperties, GameConstants.DefaultPlacingFace, _rotation);
        protected Baseplate Baseplate => _baseplate;

        private async void Start()
        {
            var blockCreateServiceLink = ServiceLocatorObject.GetLinkWrapper<BlockCreateService>();
            while (!(_baseplate.InitStatusModule.IsInitialized && blockCreateServiceLink.CanBeResolved)) await Awaitable.FixedUpdateAsync();
            _blockProperties = BlockPresetsDepot.GetProperty(_preset, new BlockMaterial(VisualMaterialType.Plastic, _color));
            BlockModel model = await blockCreateServiceLink.Value.CreateBlockModel(_blockProperties);
            _modelTransform = model.transform;
            Redraw();
        }
        private void Redraw()
        {
            var info = PlacingInfo;
            var pinPosition = _baseplate.GetPlatePinWorldPosition(new Vector2Byte(_fitPosition));
            _rotationAxlePoint = pinPosition;

            var plane = _blockProperties.GetPlanesList().GetFitPlane(_initialPinPlane);
            var virtualBlock = new VirtualBlock(Vector3.zero, PlacingInfo);
            Vector3 localPos = _baseplate.TransformPosition(PlacingInfo.GetBlockCenterPosition(pinPosition));
            _modelTransform.position = localPos;
            _modelTransform.rotation = _baseplate.ModelsHost.rotation * info.Rotation;
            virtualBlock.Reposition(localPos);
            OnBlockPositioned(virtualBlock);
        }
        protected virtual void OnBlockPositioned(VirtualBlock block) { }
        private void Update()
        {
            if (_redraw)
            {
                _redraw = false;
                Redraw();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && enabled) Gizmos.DrawLine(_rotationAxlePoint + Vector3.down * 5f, _rotationAxlePoint + Vector3.up * 5f);
        }
#endif
    }
}
