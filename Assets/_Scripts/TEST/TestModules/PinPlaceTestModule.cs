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
        private RectDrawInfo _rectDrawInfo = null, _blockingRectDraw = null;
        private IContactPlaneController ContactPlaneController
        {
            get
            {
                if (f_contactPlaneController == null) f_contactPlaneController = Properties.CreateContactPlaneController(_contactFace);
                return f_contactPlaneController;
            }
        }
        private IContactPlaneController f_contactPlaneController;
        protected override BlockPreset BlockPreset => _preset;
        protected PlacingBlockInfo BlockInfo => new (ContactPlaneController.GetContactPinAddress(), Properties, _contactFace, _rotation);


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

            if (PinFound && _hostsManager.TryGetHost(PositionInfo.BlockHostID, out var blockHost))
            {
                if (Input.GetKeyDown(KeyCode.F)) ContactPlaneController.Move(CameraProjectedLocalVector(Vector3.left));
                if (Input.GetKeyDown(KeyCode.H)) ContactPlaneController.Move(CameraProjectedLocalVector(Vector3.right));
                if (Input.GetKeyDown(KeyCode.T)) ContactPlaneController.Move(CameraProjectedLocalVector(Vector3.forward));
                if (Input.GetKeyDown(KeyCode.G)) ContactPlaneController.Move(CameraProjectedLocalVector(Vector3.back));

                var virtualBlock = blockHost.CreateVirtualBlock(PositionInfo.StructureAddress, BlockInfo);
                var rect = Utilities.ProjectBlock(_contactFace.Inverse(), virtualBlock);
                var obstruction = blockHost.CheckZoneForObstruction(
                        PositionInfo.StructureAddress.CutPlaneID,
                        rect
                        );


                _blockingRectDraw = new RectDrawInfo(blockHost, PositionInfo.StructureAddress.CutPlaneID, rect);
                Marker.SetModelStatus(obstruction ? BlockPositionStatus.Obstructed : BlockPositionStatus.CanBePlaced);
                if (!obstruction && Input.GetMouseButtonDown(0))
                {
                    _rectDrawInfo = new RectDrawInfo(blockHost, PositionInfo.StructureAddress.CutPlaneID, rect);

                    var properties = Properties.ChangeMaterial(new BlockMaterial(VisualMaterialType.Plastic, _blockColor));
                    blockHost.TryAddDetail(PositionInfo.StructureAddress, BlockInfo.ChangeProperties(properties));
                }
            }
            else
            {
                Marker.SetModelStatus(BlockPositionStatus.CannotBePlaced);
            }

            Vector3 CameraProjectedLocalVector(Vector3 normalizedDirection) => blockHost.ModelsHost.TransformDirection(Camera.main.transform.TransformDirection(normalizedDirection));
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
            /*
            if (PinFound && _hostsManager.TryGetHost(PositionInfo.BlockHostID, out var host) )
            {
                var cutplane = host.CutPlanesDataProvider.GetCuttingPlane(PositionInfo.StructureAddress.CutPlaneID);
                Vector3 blockPos = host.TransformPosition(BlockInfo.GetBlockCenterPosition(cutplane.PlaneAddressToLocalPos(PositionInfo.StructureAddress)));

                var virtualBlock = host.CreateVirtualBlock(PositionInfo.StructureAddress, BlockInfo);
                var projection = Utilities.ProjectBlock(cutplane.Face, virtualBlock);
                Vector3 zeroPos = host.TransformPosition(projection.Position, cutplane),
                    onePos = host.TransformPosition(projection.TopRight, cutplane);

                var matrix = Matrix4x4.TRS(Vector3.zero, virtualBlock.Rotation, Vector3.one);
                Gizmos.matrix = matrix;
                Gizmos.DrawWireCube(matrix.inverse.MultiplyVector(blockPos), new Vector3(projection.Width, 1f, projection.Height)); 
            }
            */


            if (_rectDrawInfo != null)
            {
                Gizmos.color = Color.green;
                Gizmos.matrix = _rectDrawInfo.Matrix;
                Gizmos.DrawWireCube(_rectDrawInfo.WorldPos, _rectDrawInfo.Size);
            }
            if (_blockingRectDraw != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = _blockingRectDraw.Matrix;
                Gizmos.DrawWireCube(_blockingRectDraw.WorldPos, _blockingRectDraw.Size);
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

                var face = plane.Face;
                float angle = rect.Orths.Quaternion.eulerAngles.z;
       
                var rotation = host.ModelsHost.rotation * Quaternion.AngleAxis(angle, face.Normal);
                WorldPos = host.TransformPosition(rect.Center, plane);
                Size = new Vector3(rect.Width, 1f, rect.Height);
                Matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
                WorldPos = Matrix.inverse.MultiplyVector(WorldPos);
            }
        }
    }
}
