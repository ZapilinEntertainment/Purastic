using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public sealed class BlocksColliderModule : IColliderOwner
    {
        private readonly IBlocksHost _host;
        private readonly PlacedBlocksListHandler _blocksList;
        private List<int> _collidersIDs = new();
        private Dictionary<int, Collider> _collidersList = new();
        private Dictionary<int, int> _collidersHostIdentifierList = new(); // one block can have many colliders, but a collider have only one owner
        public bool HaveMultipleColliders => _collidersIDs.Count > 1;
        public int GetColliderID() => _collidersIDs[0];
        public IReadOnlyCollection<int> GetColliderIDs() => _collidersIDs;

        public BlocksColliderModule(IBlocksHost host, PlacedBlocksListHandler blocksList)
        {
            _host = host;
            _blocksList = blocksList;
            _host.CollidersHost.layer = LayerConstants.GetDefinedLayer(DefinedLayer.Pinplane);

            var blocks = _blocksList.GetPlacedBlocks();
            foreach (var block in blocks)
            {
                var collider = _host.CollidersHost.AddComponent<BoxCollider>();
                var bounds = block.Properties.ModelSize;
                collider.size = bounds;
                collider.center = 0.5f * bounds.y * Vector3.up;
                AddColliderToList(collider, block.ID);
            }

            host.OnBlockPlacedEvent += OnBlockAdded;
        }

        private void OnBlockAdded(PlacedBlock block)
        {
            var collider = _host.CollidersHost.AddComponent<BoxCollider>();
            var bounds = block.Properties.ModelSize;
            collider.size = bounds;
            collider.center = 0.5f * bounds.y * Vector3.up;
            AddColliderToList(collider, block.ID);
        }
        private void AddColliderToList(Collider collider, int blockID)
        {
            int id = collider.GetInstanceID();
            if (_collidersList.TryAdd(id, collider))
            {
                _collidersIDs.Add(id);
                _collidersHostIdentifierList.Add(id, blockID);
            }
        }

        public bool TryGetBlock(int colliderId, out int blockId) => _collidersHostIdentifierList.TryGetValue(colliderId, out blockId);
    }
}
