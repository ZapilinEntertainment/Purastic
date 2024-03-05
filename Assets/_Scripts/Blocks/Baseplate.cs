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
        public Action<int> OnBlockPlacedEvent { get; set; }

        #region collider owner
        public int GetColliderID() => _colliderModule.GetColliderID();
        public IReadOnlyCollection<int> GetColliderIDs() => _colliderModule.GetColliderIDs();
        public bool HaveMultipleColliders => _colliderModule.HaveMultipleColliders;
        #endregion

        private BlockProperties GetRootBlock() => new BlockProperties(            
            new Vector3(Width * GameConstants.BLOCK_SIZE, GameConstants.PLATE_THICK, Length * GameConstants.BLOCK_SIZE),
            _material,
            FitPlanesConfigsDepot.SaveConfig(new KnobGrid(Width, Length))
            );

        private void Start()
        {
            _placedBlocksList = new(GetRootBlock());
            _structureModule = new(_placedBlocksList);
            _visualizer = gameObject.AddComponent<ConstructionVisualizer>();
            _colliderModule = new (this, _placedBlocksList);
        }

        public bool TryPinDetail(FitPosition position, BlockProperties block)
        {
            if (_placedBlocksList.TryGetBlock(position.BlockId, out var placedBlock) 
                && placedBlock.TryFormFitInfo(position, block, out var fitInfo)
                && _structureModule.TryAddBlock( block, position, fitInfo, out var newPlacedBlock)
               )
            {
                int id = newPlacedBlock.ID;
                OnBlockPlacedEvent?.Invoke(id);
                return true;
            }
            else return false;
           
        }

        public FitPosition PointToPin(int colliderID, Vector3 point)
        {
            if (_colliderModule.TryGetBlock(colliderID, out int blockID) && _placedBlocksList.TryGetBlock(blockID, out var placedBlock))
            {
                return placedBlock.GetPinPosition(CollidersHost.transform.InverseTransformPoint(point));
            }
            else return new FitPosition(point);
        }

        public IReadOnlyCollection<BlockProperties> GetBlocks() => _placedBlocksList.GetBlocksProperties();
    }
}
