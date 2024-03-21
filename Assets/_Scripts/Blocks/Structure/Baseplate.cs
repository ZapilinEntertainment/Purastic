using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public sealed class Baseplate : MonoBehaviour, IBlocksHost
	{

		[field:SerializeField] public byte Width = 16, Length = 16;
        public int ID { get; private set; }
        public int RootBlockId => _placedBlocksList.RootBlockId;
        public ICutPlanesDataProvider CutPlanesDataProvider => _cuttingPlanesManager;
        public Transform ModelsHost => transform;
        public GameObject CollidersHost => gameObject;

        public InitStatusModule InitStatusModule { get; set; }


        [SerializeField] private BlockMaterial _material = new BlockMaterial();
        private Container _modulesContainer;
		private ConstructionVisualizerModule _visualizer;
        private BlocksColliderModule _colliderModule;
        private BlockStructureModule _structureModule;
        private PlacedBlocksListHandler _placedBlocksList;
        private CuttingPlanesManager _cuttingPlanesManager; // contains all info about block pin surfaces
        private IBlocksHost BlocksHost => this;
        public PlacedBlock RootBlock => _placedBlocksList.RootBlock;
        public Action<PlacedBlock> OnBlockPlacedEvent { get; set; }

        #region collider owner
        public int GetColliderID() => _colliderModule.GetColliderID();
        public IReadOnlyCollection<int> GetColliderIDs() => _colliderModule.GetColliderIDs();
        public bool HaveMultipleColliders => _colliderModule.HaveMultipleColliders;

        #endregion
        
        private BlockProperties GetRootBlockProperties() {
            return new BlockProperties(new FitsGridConfig(FitType.Knob, Width, Length), _material, 1);
        }
        private void Awake()
        {
            InitStatusModule = new(); // C#9 {init;} not works
        }
        private void Start()
        {
            ID = ServiceLocatorObject.Get<BlockHostsManager>().Register(this);

            _modulesContainer = ServiceLocatorObject.Instance.ReserveAndGetContainer();
            _modulesContainer.RegisterInstance(this as IBlocksHost);
            
            _structureModule = new(_modulesContainer);
            _visualizer = gameObject.AddComponent<ConstructionVisualizerModule>();
            _visualizer.Setup(_modulesContainer);
            _colliderModule = new (_modulesContainer);
            _cuttingPlanesManager = new CuttingPlanesManager(_modulesContainer);

            var initialBlock = new VirtualBlock(Vector3.zero, new PlacingBlockInfo(GetRootBlockProperties()));
            _placedBlocksList = new( initialBlock,_modulesContainer); 
            OnBlockPlacedEvent?.Invoke(_placedBlocksList.RootBlock);

            ServiceLocatorObject.Get<ColliderListSystem>().AddBlockhost(this);

            InitStatusModule.OnInitialized();
            // test view:
            //var virtualBlock = new VirtualBlock(GetPlatePinPosition(Vector2Byte.one * 2), new PlacingBlockInfo(BlockPresetsDepot.GetProperty(BlockPreset.StandartBrick_1x1, new BlockMaterial(VisualMaterialType.Plastic, BlockColor.Lavender))));
            //OnBlockPlacedEvent?.Invoke(new PlacedBlock(-1,virtualBlock));   
        }

        public bool CheckZoneForObstruction(int cutPlaneID, AngledRectangle rect)
        {
            if (_cuttingPlanesManager.TryGetLockZone(cutPlaneID, out var lockZone))
            {
                return lockZone.CheckZoneForLockers(rect);
            }
            else return false;
        }
        public bool TryAddDetail(FitElementStructureAddress pinStructureAddress, PlacingBlockInfo placingBlockInfo)
        {
            if (  _placedBlocksList.TryGetBlock(pinStructureAddress.BlockID, out var baseBlock) && _cuttingPlanesManager.TryGetCuttingPlane(pinStructureAddress, out var plane)) 
            {
                /*
                Vector3 predictedLocalContactPoint = plane.PlaneAddressToLocalPos(pinStructureAddress);
                Debug.Log($"{pinStructureAddress.PlaneAddress} -> {predictedLocalContactPoint}");
                var virtualBlock = new VirtualBlock(placingBlockInfo.GetBlockCenterPosition(predictedLocalContactPoint) , placingBlockInfo);
                */

                var virtualBlock = CreateVirtualBlock(pinStructureAddress.PlaneAddress.PinIndex, placingBlockInfo);

                if (_cuttingPlanesManager.TryConnectNewBlock(baseBlock, pinStructureAddress, virtualBlock, out var connectedPins))
                {
                    _structureModule.AddBlock(baseBlock, pinStructureAddress, virtualBlock, connectedPins, out var newPlacedBlock);
                    var blockLink = newPlacedBlock;
                    OnBlockPlacedEvent?.Invoke(blockLink);
                    return true;
                }
            }
            return false;
        }
        public bool TryGetFitElementPosition(RaycastHit hit, out FoundedFitElementPosition position)
        {
            if (_colliderModule.TryGetBlock(hit.colliderInstanceID, out int blockID) && _placedBlocksList.TryGetBlock(blockID, out var placedBlock))
            {
                Vector3 localHitPos = ModelsHost.InverseTransformPoint(hit.point);
                return _cuttingPlanesManager.TryGetFitElementPosition(localHitPos, hit.normal, placedBlock, out position) ;
            }
            position = default;
            return false;
        }

        public Vector3 GetPlatePinWorldPosition(Vector2Byte index)
        {
            var rootBlock = _placedBlocksList.RootBlock;
            var plane = rootBlock.Properties.GetPlanesList().GetFitPlane(0);
            var facePos = plane.GetFaceSpacePosition(index);
            Vector3 modelPos = rootBlock.FacePositionToModelPosition(facePos, BlockFaceDirection.Up);
            return BlocksHost.TransformPosition( modelPos);
        }
        public ICuttingPlane GetPlatePlane()
        {
            var rootBlock = _placedBlocksList.RootBlock;
            return _cuttingPlanesManager.GetOrCreateCutPlane(new CuttingPlanePosition(BlockFaceDirection.Up, rootBlock.GetFaceZeroPointInBlockSpace(BlockFaceDirection.Up).y));
        }
        public IReadOnlyCollection<BlockProperties> GetBlocks() => _placedBlocksList.GetBlocksProperties();
        public bool TryFormPlateAddress(Vector2Byte index, out FitElementStructureAddress address)
        {
            var face = BlockFaceDirection.Up;
            if (_cuttingPlanesManager.TryGetCuttingPlane(_placedBlocksList.RootBlock, face, out var plane))
            {
                address = new FitElementStructureAddress(
                RootBlockId,
                plane.ID,
                face,
                new FitElementPlaneAddress(index)
                );
                return true;
            }
            else
            {
                address = default;
                return false;
            }
        }

        private void OnDestroy()
        {
            ServiceLocatorObject.s_ReleaseContainer(_modulesContainer.ID);
            if (ServiceLocatorObject.TryGet<ColliderListSystem>(out var link)) link.RemoveBlockhost(this);
            if (ServiceLocatorObject.TryGet<BlockHostsManager>(out var manager)) manager.Unregister(this);
        }

        public Vector3 InverseTransformPosition(Vector3 position) => ModelsHost.InverseTransformPoint(position);
        public Vector3 TransformPosition(Vector3 position) => ModelsHost.TransformPoint(position);


        public void LockPins(IReadOnlyCollection<ConnectingPin> pins)
        {
            var platePlane = GetPlatePlane();
            _cuttingPlanesManager.AddLockZones(platePlane.ID, pins);
        }
        public void LockPlateZone(AngledRectangle rect, out IReadOnlyCollection<ConnectingPin> lockedPins) {
            var platePlane = GetPlatePlane();
            lockedPins= platePlane.GetLandingPinsList(rect).Pins;
            _cuttingPlanesManager.AddLockZones(platePlane.ID, lockedPins );
        }
        public void UnlockPlateZone(IReadOnlyCollection<ConnectingPin> lockedPins) {
            var platePlane = GetPlatePlane();
            _cuttingPlanesManager.RemoveLocks(platePlane.ID, lockedPins);
        }

        public VirtualBlock CreateVirtualBlock(Vector2Byte fitPosition, PlacingBlockInfo placingInfo)
        {
            var pinPosition = GetPlatePinWorldPosition(fitPosition);
            Vector3 localPos = TransformPosition(placingInfo.GetBlockCenterPosition(pinPosition));
            return new VirtualBlock(localPos, placingInfo);
        }
        public VirtualBlock CreateVirtualBlock(Vector2Byte fitPosition, PlacingBlockInfo placingInfo, out Vector3 pinPosition)
        {
            pinPosition = GetPlatePinWorldPosition(fitPosition);
            Vector3 localPos = TransformPosition(placingInfo.GetBlockCenterPosition(pinPosition));
            return new VirtualBlock(localPos, placingInfo);
        }
    }
}
