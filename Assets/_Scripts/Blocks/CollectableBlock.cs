using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class CollectableBlock : PlayerTrigger, IContainable, ICollectable
	{
        [SerializeField]private BlockMaterial _blockMaterial;
        [SerializeField] private Vector3Int _dimensions;
        private BlockModel _model;
        public bool IsConsumable => false;
        public int ContainerID { get; set; }


         
        
        protected override async void Start()
        {
            base.Start();
           var blockCreateService = await ServiceLocatorObject.GetWhenLinkReady<BlockCreateService>();

            var block = new BlockProperties(new FitsGridConfig(FitType.Knob, _dimensions.x, _dimensions.z), _blockMaterial, _dimensions.y);
            _model = await blockCreateService.CreateBlockModel(block);
            _model.transform.SetParent(transform, false);
        }
        

        protected override void OnPlayerEnter(PlayerController player)
        {
            base.OnPlayerEnter(player);
            if (player.TryCollect(this))
            {
                Dispose();
            }
        }
        public bool TryGetEquippable(out IEquippable equippable)
        {
            if (_model != null)
            {
                equippable = _model;
                _model.transform.parent = null;
                _model = null;
                return true;
            }
            else
            {
                equippable = null;
                return false;
            }
        }

        override public void Dispose()
        {            
            if (_model != null && ServiceLocatorObject.TryGet<BlockModelPoolService>(out var cacheService))
            {
                cacheService.CacheModel(_model);
                _model = null;
            }
            base.Dispose();
        }

        public class Factory : ContainerObjectFactory<CollectableBlock>
        {
            public Factory(Container container) : base(container)
            {
            }

            public CollectableBlock Create(Vector3 Position, BlockMaterial material, Vector3Int dimensions)
            {
                var block = Instantiate();
                block._blockMaterial = material;
                block._dimensions = dimensions;
                block.transform.position = Position;
                return block;
            }
            protected override CollectableBlock Instantiate()
            {
                var obj = new GameObject("collectable");
                obj.AddComponent<SphereCollider>().radius = 1f;
                return obj.AddComponent<CollectableBlock>();
            }
        }
    }
}
