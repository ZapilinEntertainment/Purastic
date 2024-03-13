using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinPlaceTestModule : PinDefineTestModule
	{
        [SerializeField] private BlockPreset _preset = BlockPreset.StandartBrick_1x1;
        [SerializeField] private BlockFaceDirection _contactFace = GameConstants.DefaultPlacingFace;
        [SerializeField] private Vector2Byte _zeroPin;
        [SerializeField] private PlacedBlockRotation _rotation = PlacedBlockRotation.NoRotation;
        [SerializeField] private BlockColor _blockColor = BlockColor.Amber;
        private BlockHostsManager _hostsManager;
        protected override BlockPreset BlockPreset => _preset;
        protected PlacingBlockInfo BlockInfo => new (_zeroPin, _properties, _contactFace, _rotation);


        protected override void AfterStart()
        {
            _hostsManager = ServiceLocatorObject.Get<BlockHostsManager>();
        }
        protected void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
               _rotation = _rotation.RotateRight();
            }
            if (PinFound)
            {
                if (Input.GetMouseButtonDown(0) && _hostsManager.TryGetHost(FitPosition.BlockHostID, out var blockHost))
                {
                    var info = BlockInfo;
                    var properties = _properties.ChangeMaterial(new BlockMaterial(VisualMaterialType.Plastic, _blockColor));
                    blockHost.TryAddDetail(FitPosition.StructureAddress, BlockInfo.ChangeProperties(properties));
                }
            }
        }
        override protected void PositionMarker(Vector3 contactPos)
        {
            Vector3 pos = BlockInfo.GetBlockCenterPosition(contactPos);
            Marker.transform.position = pos;
            Marker.transform.rotation = _rotation.Quaternion;
        }        
    }
}
