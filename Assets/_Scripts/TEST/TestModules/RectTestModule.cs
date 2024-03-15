using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public sealed class RectTestModule : MonoBehaviour
	{
        [SerializeField] private bool _redraw = false;
		[SerializeField] private Baseplate _baseplate;
        [SerializeField] private Vector2Int _fitPosition, _initialPin;
        [SerializeField] private BlockPreset _blockPreset;
        [SerializeField] private RotationStep _rotationStep;
        [SerializeField] private int _rotationValue;
        private Transform _model;
        private IReadOnlyCollection<ConnectingPin> _lockedPins = null;

        private async void Start()
        {
            while (!_baseplate.InitStatusModule.IsInitialized) await Awaitable.FixedUpdateAsync();
            _model = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            SetPosition();
        }
        private void SetPosition()
        {
            var plane = _baseplate.GetPlatePlane();
            Vector2Byte fitPoint = new(_fitPosition) ;
            var pinAddress = new FitElementPlaneAddress(0, fitPoint);
            var property = BlockPresetsDepot.GetProperty(_blockPreset, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.DefaultWhite));
            var rotation = new Rotation2D(_rotationStep, _rotationValue);
            var placeInfo = new PlacingBlockInfo(
                new Vector2Byte(_initialPin),
                property,
                BlockFaceDirection.Down,
                new PlacedBlockRotation(rotation, Rotation2D.NoRotation)
                );

            //Debug.Log(pinAddress);
            Vector2 pinCutPlanePos = plane.PlaneAddressToCutPlanePos(pinAddress);
            //Debug.Log(pinCutPlanePos);
            Vector3 pinZeroPos = plane.CutPlaneToLocalPos(pinCutPlanePos);
            //Debug.Log(pinZeroPos);

            Vector3 blockPos = placeInfo.GetBlockCenterPosition(pinZeroPos);

            var virtualBlock = new VirtualBlock(
                blockPos,
                placeInfo
                );

            //Debug.Log($"center: {virtualBlock.LocalPosition}, corner: {virtualBlock.GetFaceZeroPointInLocalSpace(BlockFaceDirection.Up)}");

            //Debug.Log(virtualBlock.Properties.ModelSize);
            var rect = Utilities.ProjectBlock(plane.Face, virtualBlock,false);
            DrawVirtualBlock(virtualBlock);
            //DrawRect(rect, plane);
            if (_lockedPins != null) _baseplate.UnlockPlateZone(_lockedPins);
            _baseplate.LockPlateZone(rect, out _lockedPins);
        }
        private void DrawRect(AngledRectangle arect, ICuttingPlane plane)
        {
            Vector3 pos = plane.CutPlaneToLocalPos(arect.Rect.center);
            _model.transform.position = _baseplate.TransformPosition(pos);
            _model.transform.localScale = new Vector3(arect.Rect.width, 1f, arect.Rect.height);
            //_model.transform.rotation = _baseplate.ModelsHost.rotation * plane.Face.ToRotation();
        }
        private void DrawVirtualBlock(VirtualBlock block)
        {
            _model.transform.position = _baseplate.TransformPosition(block.LocalPosition);
            _model.transform.localScale = new Vector3(block.Properties.ModelSize.x, 1f, block.Properties.ModelSize.z);
            _model.transform.rotation = _baseplate.ModelsHost.rotation * block.Rotation.Quaternion;
        }

        private void Update()
        {
            if (_redraw)
            {
                _redraw = false;
                SetPosition();

            }
        }
    }
}
