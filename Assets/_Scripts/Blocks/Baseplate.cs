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
        public GameObject CollidersHost => gameObject;
        

        [SerializeField] private BlockMaterial _material = new BlockMaterial();
		private ConstructionVisualizer _visualizer;
        private BlocksColliderModule _colliderModule;

        #region collider owner
        public int GetColliderID() => _colliderModule.GetColliderID();
        public int[] GetColliderIDs() => _colliderModule.GetColliderIDs();
        public bool HaveMultipleColliders => _colliderModule.HaveMultipleColliders;
        #endregion

        public Block ToBlock() => new Block(
            new KnobGrid(Width, Length),
            new Vector3(Width * GameConstants.BLOCK_SIZE, GameConstants.PLATE_THICK, Length * GameConstants.BLOCK_SIZE),
            _material
            );

        private void Start()
        {
            _visualizer = new ConstructionVisualizer(this);
            _colliderModule = new BlocksColliderModule(this);
        }

        public bool TryPinDetail(PinPosition position, Block block)
        {
            throw new NotImplementedException();
        }

        public PinPosition PointToPin(Vector3 point)
        {
            throw new NotImplementedException();
        }
    }
}
