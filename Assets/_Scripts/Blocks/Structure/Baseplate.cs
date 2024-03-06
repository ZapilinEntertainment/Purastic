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
            return new BlockProperties(new FitsGridConfig(FitType.Knob, Width, Length), _material, 1);
        }

        private void Start()
        {
            _placedBlocksList = new(GetRootBlock());
            _structureModule = new(_placedBlocksList);
            _visualizer = gameObject.AddComponent<ConstructionVisualizer>();
            _colliderModule = new (this, _placedBlocksList);
            _cuttingPlanesManager = new CuttingPlanesManager(this, _placedBlocksList);
        }

        public bool TryPinDetail(FitElementStructureAddress pinStructureAddress, BlockProperties newBlockProperties)
        {
            if (_placedBlocksList.TryGetBlock(pinStructureAddress.BlockID, out var placedBlock) 
                && _cuttingPlanesManager.TryConnectNewBlock(placedBlock, pinStructureAddress, newBlockProperties.GetPlanesList(), out var connectedPins)
                && _structureModule.TryAddBlock( newBlockProperties, pinStructureAddress, connectedPins, out var newPlacedBlock)
               )
            {
                var blockLink = newPlacedBlock;
                OnBlockPlacedEvent?.Invoke(blockLink);
                return true;
            }
            else return false;
           
        }

        public bool TryGetFitElementPosition(int colliderID, Vector3 point, out FitElementStructureAddress position)
        {
            if (_colliderModule.TryGetBlock(colliderID, out int blockID) && _placedBlocksList.TryGetBlock(blockID, out var placedBlock))
            {
                Vector3 localHitPos = ModelsHost.InverseTransformPoint(point);                
                return _cuttingPlanesManager.TryGetFitElementPosition(localHitPos, placedBlock, out position);
            }
            position = default;
            return false;
        }

        public IReadOnlyCollection<BlockProperties> GetBlocks() => _placedBlocksList.GetBlocksProperties();
    }
}
