using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
	public class CollectableCaller : NetworkBehaviour
	{
        [SerializeField] private BlockMaterial _blockMaterial;
        [SerializeField] private Vector3Int _dimensions;

        private void Start()
        {
            var factory = ServiceLocatorObject.GetFactory<CollectableBlock.Factory>();
            factory.Create(transform.position, _blockMaterial, _dimensions);
        }
    }
}
