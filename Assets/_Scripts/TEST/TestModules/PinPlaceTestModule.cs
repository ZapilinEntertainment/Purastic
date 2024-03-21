using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinPlaceTestModule : PinDefineTestModule
	{
        [SerializeField] private BlockPreset _preset = BlockPreset.StandartBrick_1x1;
        [SerializeField] private BlockFaceDirection _contactFace = GameConstants.DefaultPlacingFace;
        [SerializeField] private Vector2Int _zeroPin;
        [SerializeField] private Quaternion _rotation = Quaternion.identity;
        [SerializeField] private BlockColor _blockColor = BlockColor.Amber;
        private BlockHostsManager _hostsManager;
        private RectDrawInfo _rectDrawInfo = null;
        private FitElementPlaneAddress ConnectionPin => new FitElementPlaneAddress(1, new Vector2Byte(_zeroPin));
        protected override BlockPreset BlockPreset => _preset;
        protected PlacingBlockInfo BlockInfo => new (ConnectionPin, _properties, _contactFace, _rotation);


        protected override void AfterStart()
        {
            _hostsManager = ServiceLocatorObject.Get<BlockHostsManager>();
        }
        protected void Update()
        {
            if (!IsReady) return;
            if (Input.GetMouseButtonDown(1))
            {
               _rotation *= Quaternion.AngleAxis(90f, Vector3.up);
            }
            if (PinFound)
            {
                
                if (Input.GetMouseButtonDown(0) && _hostsManager.TryGetHost(FitPosition.BlockHostID, out var blockHost))
                {
                    var virtualBlock = new VirtualBlock(FitPosition.WorldPoint.Position, BlockInfo);
                    var rect = Utilities.ProjectBlock(_contactFace.Inverse(), virtualBlock);
                    _rectDrawInfo = new RectDrawInfo(blockHost, FitPosition.StructureAddress.CutPlaneID, rect);

                    bool obstruction =blockHost.CheckZoneForObstruction(FitPosition.BlockHostID, rect);
                    Marker.SetModelStatus(obstruction ? BlockPositionStatus.Obstructed : BlockPositionStatus.CanBePlaced);

                    var properties = _properties.ChangeMaterial(new BlockMaterial(VisualMaterialType.Plastic, _blockColor));
                    blockHost.TryAddDetail(FitPosition.StructureAddress, BlockInfo.ChangeProperties(properties));
                }
                else
                {
                    Marker.SetModelStatus(FitPosition.PositionIsObstructed ? BlockPositionStatus.Obstructed : BlockPositionStatus.CanBePlaced);
                }
            }
            else Marker.SetModelStatus(BlockPositionStatus.CannotBePlaced);
        }
        override protected void PositionMarker(Vector3 contactPos)
        {
            Vector3 pos = BlockInfo.GetBlockCenterPosition(contactPos);
            Marker.Model.SetPoint(pos, _rotation);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (PinFound && _hostsManager.TryGetHost(FitPosition.BlockHostID, out var host) )
            {
                var cutplane = host.CutPlanesDataProvider.GetCuttingPlane(FitPosition.StructureAddress.CutPlaneID);
                Vector3 blockPos = BlockInfo.GetBlockCenterPosition(cutplane.PlaneAddressToLocalPos(ConnectionPin));
                var virtualBlock = new VirtualBlock(blockPos, BlockInfo);
                var projection = Utilities.ProjectBlock(cutplane.Face, virtualBlock);
                Gizmos.DrawWireCube( blockPos, new Vector3(projection.Width,1f, projection.Height));
            }


            if (_rectDrawInfo != null)
            {
                Gizmos.color = Color.green;
                Gizmos.matrix = _rectDrawInfo.Matrix;
                Gizmos.DrawWireCube(_rectDrawInfo.WorldPos, _rectDrawInfo.Size);
            }
        }
#endif

        private class RectDrawInfo
        {
            public readonly Vector3 WorldPos;
            public readonly Vector3 Size;
            public readonly Matrix4x4 Matrix;

            public RectDrawInfo(IBlocksHost host, int cutPlaneId, AngledRectangle rect)
            {
                var plane = host.CutPlanesDataProvider.GetCuttingPlane(cutPlaneId);
                Vector3 localPos = plane.CutPlaneToLocalPos(rect.Position);                
                Size = new Vector3(rect.Width, 1f, rect.Height);

                var rotation = host.ModelsHost.rotation * rect.Orths.Quaternion;
                WorldPos = host.TransformPosition(localPos) + (0.5f * Size);
                Matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            }
        }
    }
}
