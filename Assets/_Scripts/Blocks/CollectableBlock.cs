using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class CollectableBlock : PlayerTrigger, IContainable, ICollectable
	{
        [SerializeField] private Block _block;
        private BlockModel _model;
        public bool IsConsumable => false;
        public int ContainerID { get; set; }
        private async void Start()
        {
           var blockCreateService = await ServiceLocatorObject.GetWhenLinkReady<BlockCreateService>();
            _model = await blockCreateService.CreateBlockModel(_block);
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
            if (_model != null && ServiceLocatorObject.TryGet<BlockModelCacheService>(out var cacheService))
            {
                cacheService.CacheModel(_model);
                _model = null;
            }
            base.Dispose();
        }
    }
}
