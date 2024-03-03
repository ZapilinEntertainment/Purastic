using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public sealed class BlocksColliderModule : IColliderOwner
    {
        private readonly IBlocksHost _host;
        private List<Collider> _colliders = new ();
        public bool HaveMultipleColliders => _colliders.Count > 1;
        public int GetColliderID() => _colliders[0].GetInstanceID();
        public int[] GetColliderIDs()
        {
            int count = _colliders.Count;
            var ids = new int[count];
            for (int i = 0; i < count; i++)
            {
                ids[i] = _colliders[i].GetInstanceID();
            }
            return ids;
        }

        public BlocksColliderModule(Baseplate host)
        {
            _host = host;
            _host.CollidersHost.layer = LayerConstants.GetDefinedLayer(DefinedLayer.Pinplane);

            var baseBlock = host.ToBlock();
            var collider = _host.CollidersHost.AddComponent<BoxCollider>();
            collider.size = baseBlock.Size;
            collider.center = 0.5f * baseBlock.Size.y * Vector3.up;
            _colliders.Add(collider);
        }
    }
}
