using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public sealed class Baseplate : MonoBehaviour, IBlocksHost
	{

		[field:SerializeField] public byte Width = 16, Length = 16;
        public int ID => GetInstanceID();
        public int RootBlockId => _placedBlocksList.RootBlockId;
        public Transform ModelsHost => transform;
        public GameObject CollidersHost => gameObject;


        [SerializeField] private BlockMaterial _material = new BlockMaterial();
		private ConstructionVisualizer _visualizer;
        private BlocksColliderModule _colliderModule;
        private BlockStructureModule _structureModule;
        private PlacedBlocksListHandler _placedBlocksList;
        private CuttingPlanesManager _cuttingPlanesManager; // contains all info about block pin surfaces
        public Action<PlacedBlock> OnBlockPlacedEvent { get; set; }

        #region collider owner
        public int GetColliderID() => _colliderModule.GetColliderID();
        public IReadOnlyCollection<int> GetColliderIDs() => _colliderModule.GetColliderIDs();
        public bool HaveMultipleColliders => _colliderModule.HaveMultipleColliders;
        #endregion

        private BlockProperties GetRootBlock() {
            return new BlockProperties(new BaseplateConfig(Width, Length), _material, 1);
        }

        private void Start()
        {
            var rootBlock = GetRootBlock();

            _placedBlocksList = new(new PlacingBlockInfo(rootBlock,PlacedBlockRotation.NoRotation));
            _structureModule = new(_placedBlocksList);
            _visualizer = gameObject.AddComponent<ConstructionVisualizer>();
            _visualizer.Setup(this, _placedBlocksList);
            _colliderModule = new (this, _placedBlocksList);
            _cuttingPlanesManager = new CuttingPlanesManager(this, _placedBlocksList, _structureModule);

            ServiceLocatorObject.Get<ColliderListSystem>().AddBlockhost(this);
        }

        public bool TryAddDetail(FitElementStructureAddress pinStructureAddress, PlacingBlockInfo placingBlockInfo)
        {
            if (  _placedBlocksList.TryGetBlock(pinStructureAddress.BlockID, out var baseBlock)
             &&   _cuttingPlanesManager.TryConnectNewBlock(baseBlock, pinStructureAddress, placingBlockInfo, out var connectedPins)
               )
            {
                _structureModule.AddBlock(baseBlock, pinStructureAddress,placingBlockInfo, connectedPins, out var newPlacedBlock);
                var blockLink = newPlacedBlock;
                OnBlockPlacedEvent?.Invoke(blockLink);
                return true;
            }
            else
            {                
                return false;
            }
           
        }

        public bool TryGetFitElementPosition(int colliderID, Vector3 point, out FitElementStructureAddress position)
        {
            if (_colliderModule.TryGetBlock(colliderID, out int blockID) && _placedBlocksList.TryGetBlock(blockID, out var placedBlock))
            {
               // Debug.Log("here");
                Vector3 localHitPos = ModelsHost.InverseTransformPoint(point);
                return _cuttingPlanesManager.TryGetFitElementPosition(localHitPos, placedBlock, out position);
            }
            position = default;
            return false;
        }

        public IReadOnlyCollection<BlockProperties> GetBlocks() => _placedBlocksList.GetBlocksProperties();
    }
}
