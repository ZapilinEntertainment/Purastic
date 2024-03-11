using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public sealed class PositionsTestModule : MonoBehaviour
    {
        [System.Serializable]
        private struct RotationSettings
        {
            public RotationStep Rotation;
            public sbyte Step;
        }

        [SerializeField] private bool _update = false;        
        [SerializeField] private Vector2Int _spawnPoint, _initialPin;
        [SerializeField] private RotationSettings _horizontalRotation, _verticalRotation;
        [SerializeField] private BlockPreset _blockPreset = BlockPreset.StandartBrick_2x4;
        [SerializeField] private Baseplate _basePlate;
        private GameObject _markersHost;
        private BrickModelsPack _modelsPack;
        private BlockProperties _properties;

        private void Start()
        {
            _properties = BlockPresetsDepot.GetProperty(_blockPreset, new BlockMaterial());
            ServiceLocatorObject.GetWhenLinkReady<BrickModelsPack>(OnPackLinkReady);

           // var virtualBlock = new VirtualBlock(Vector3.zero, PlacedBlockRotation.NoRotation, _properties);
           // Debug.Log(virtualBlock.FacePositionToModelPosition(Vector2.zero, BlockFaceDirection.Up));
        }
        private async void OnPackLinkReady(BrickModelsPack pack)
        {
            _modelsPack = pack;
            while (!_basePlate.InitStatusModule.IsInitialized)
            {
                await Awaitable.WaitForSecondsAsync(0.1f);
            }

            //Debug.Log(_basePlate.GetPlatePinPosition(Vector2Byte.zero));

            Redraw();
        }
        private void Redraw()
        {
            Vector3 pos = _basePlate.GetPlatePinPosition(new Vector2Byte(_spawnPoint));
            DrawBlock(pos);
        }

        private void DrawBlock(Vector3 pinPosition)
        {
            var placingInfo = new PlacingBlockInfo(new Vector2Byte(_initialPin), _properties, BlockFaceDirection.Down);
            var blockPos =  _basePlate.TransformPosition(placingInfo.GetBlockCenterPosition(pinPosition));

            var rotation = new PlacedBlockRotation(new Rotation2D(_horizontalRotation.Rotation, _horizontalRotation.Step), new Rotation2D(_verticalRotation.Rotation, _verticalRotation.Step));
            var virtualBlock = new VirtualBlock(blockPos, placingInfo);

            _markersHost = new GameObject();
            _markersHost.transform.position = blockPos;
            DrawFace(virtualBlock,BlockFaceDirection.Up);
            DrawFace(virtualBlock,BlockFaceDirection.Down);
        }
        private void DrawFace(VirtualBlock block, BlockFaceDirection face)
        {
            Quaternion faceRotation = face.ToRotation(), blockRotation = block.Rotation.Quaternion;
            var landingPoints = block.GetAllConnectionPins(face);

            foreach (var point in landingPoints)
            {
                Vector3 pos = block.FacePositionToModelPosition(point.FitElement.Position, face);
                Instantiate( _modelsPack.GetFitElementPrefab(point.FitType), pos,faceRotation, _markersHost.transform );
            }

        }

        private void FixedUpdate()
        {
            if (_update)
            {
                _update = false;
                Destroy(_markersHost);
                Redraw();
            }
        }

    }
    
}
