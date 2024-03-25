using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinPlaceTestModule : BlockPositionHighlightTestModule
	{
        [SerializeField] private BlockPreset _preset = BlockPreset.StandartBrick_1x1;
        [SerializeField] private BlockFaceDirection _contactFace = GameConstants.DefaultPlacingFace;
        [SerializeField] private Vector2Int _zeroPin;
        [SerializeField] private Quaternion _rotation = Quaternion.identity;
        [SerializeField] private BlockColor _blockColor = BlockColor.Amber;
        private BlockHostsManager _hostsManager;
        private RectDrawer _rectDrawInfo = null, _blockingRectDraw = null;
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


                var defaultAddress = PositionInfo.StructureAddress;
                var virtualBlock = blockHost.CreateVirtualBlock(defaultAddress, BlockInfo);
                var rect = Utilities.ProjectBlock(_contactFace.Mirror(), virtualBlock);
                var obstruction = blockHost.CheckZoneForObstruction(
                        PositionInfo.StructureAddress.CutPlaneID,
                        rect
                        );


                _blockingRectDraw = RectDrawer.CreateRectDrawer(blockHost, PositionInfo.StructureAddress.CutPlaneID, rect, Color.yellow);
                Marker.SetModelStatus(obstruction ? BlockPositionStatus.Obstructed : BlockPositionStatus.CanBePlaced);
                if (!obstruction && Input.GetMouseButtonDown(0))
                {
                    _rectDrawInfo = RectDrawer.CreateRectDrawer(blockHost, PositionInfo.StructureAddress.CutPlaneID, rect, Color.green);

                    var properties = Properties.ChangeMaterial(new BlockMaterial(VisualMaterialType.Plastic, _blockColor));
                    blockHost.TryAddDetail(PositionInfo.StructureAddress, BlockInfo.ChangeProperties(properties));
                }
            }
            else
            {
                Marker.SetModelStatus(BlockPositionStatus.CannotBePlaced);
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(PositionInfo.StructureAddress.ContactFace);
                }
            }

            Vector3 CameraProjectedLocalVector(Vector3 normalizedDirection) => blockHost.ModelsHost.TransformDirection(Camera.main.transform.TransformDirection(normalizedDirection));
        }
        override protected void PositionMarker(Vector3 contactPos)
        {
            Vector3 pos = BlockInfo.GetBlockCenterPosition(PositionInfo.WorldPoint.Position);
            Marker.Model.SetPoint(pos, _rotation);
        }


        protected override void OnDrawPinGizmos()
        {
            base.OnDrawPinGizmos();
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
            _rectDrawInfo?.DrawRect();
            _blockingRectDraw?.DrawRect();
        }
    }
}
