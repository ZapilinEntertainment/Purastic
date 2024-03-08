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
        public Vector3 ZeroPoint => BlocksHost.TransformPosition(new Vector3(-0.5f * Width, 0f, -0.5f * Length));        
        public Transform ModelsHost => transform;
        public GameObject CollidersHost => gameObject;

        public InitStatusModule InitStatusModule { get; set; }


        [SerializeField] private BlockMaterial _material = new BlockMaterial();
        private Container _modulesContainer;
		private ConstructionVisualizer _visualizer;
        private BlocksColliderModule _colliderModule;
        private BlockStructureModule _structureModule;
        private PlacedBlocksListHandler _placedBlocksList;
        private CuttingPlanesManager _cuttingPlanesManager; // contains all info about block pin surfaces
        private IBlocksHost BlocksHost => this;
        public Action<PlacedBlock> OnBlockPlacedEvent { get; set; }

        #region collider owner
        public int GetColliderID() => _colliderModule.GetColliderID();
        public IReadOnlyCollection<int> GetColliderIDs() => _colliderModule.GetColliderIDs();
        public bool HaveMultipleColliders => _colliderModule.HaveMultipleColliders;

        #endregion
        
        private BlockProperties GetRootBlock() {
            return new BlockProperties(new BaseplateConfig(Width, Length), _material, 1);
        }
        private void Awake()
        {
            InitStatusModule = new(); // C#9 {init;} not works
        }
        private void Start()
        {

            var rootBlock = GetRootBlock();

            _modulesContainer = ServiceLocatorObject.Instance.ReserveAndGetContainer();
            _modulesContainer.RegisterInstance(this as IBlocksHost);
            
            _structureModule = new(_modulesContainer);
            _visualizer = gameObject.AddComponent<ConstructionVisualizer>();
            _visualizer.Setup(_modulesContainer);
            _colliderModule = new (_modulesContainer);
            _cuttingPlanesManager = new CuttingPlanesManager(_modulesContainer);
            _placedBlocksList = new(new PlacingBlockInfo(rootBlock, PlacedBlockRotation.NoRotation), _modulesContainer);
            OnBlockPlacedEvent?.Invoke(_placedBlocksList.RootBlock);

            ServiceLocatorObject.Get<ColliderListSystem>().AddBlockhost(this);

            InitStatusModule.OnInitialized();
        }



        public bool TryAddDetail(FitElementStructureAddress pinStructureAddress, PlacingBlockInfo placingBlockInfo)
        {
            if (  _placedBlocksList.TryGetBlock(pinStructureAddress.BlockID, out var baseBlock) && _cuttingPlanesManager.TryGetCuttingPlane(pinStructureAddress, out var plane)) 
            {
                Vector2 predictedLocalContactPoint = plane.PlaneAddressToCutPlanePos(pinStructureAddress.PlaneAddress);
                Vector3 localPos = plane.PlaneAddressToLocalPos(pinStructureAddress,BlocksHost);
                Debug.Log(BlocksHost.TransformPosition(localPos));
                var virtualBlock = new VirtualBlock(localPos, placingBlockInfo);
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
        public bool TryGetFitElementPosition(int colliderID, Vector3 point, out FoundedFitElementPosition position)
        {
            if (_colliderModule.TryGetBlock(colliderID, out int blockID) && _placedBlocksList.TryGetBlock(blockID, out var placedBlock))
            {
               // Debug.Log("here");
                Vector3 localHitPos = ModelsHost.InverseTransformPoint(point);
                if ( _cuttingPlanesManager.TryGetFitElementPosition(localHitPos, placedBlock, out var structureAddress))
                {
                    position = new FoundedFitElementPosition(structureAddress, new VirtualPoint(point, ModelsHost.rotation));
                }
            }
            position = default;
            return false;
        }

        public IReadOnlyCollection<BlockProperties> GetBlocks() => _placedBlocksList.GetBlocksProperties();
        public FitElementStructureAddress FormPlateAddress(Vector2Byte index)
        {
            return new FitElementStructureAddress(
                RootBlockId,
                _cuttingPlanesManager.GetUpLookingPlane().ID,
                new BlockFaceDirection(FaceDirection.Up),
                new FitElementPlaneAddress(index)
                );
        }

        private void OnDestroy()
        {
            ServiceLocatorObject.s_ReleaseContainer(_modulesContainer.ID);
        }
    }
}
