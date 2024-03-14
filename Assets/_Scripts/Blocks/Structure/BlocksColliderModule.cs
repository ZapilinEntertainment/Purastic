using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public sealed class BlocksColliderModule : SubcontainerModule, IColliderOwner
    {
        private IBlocksHost BlocksHost => _localResolver.Item1;
        private PlacedBlocksListHandler BlocksList => _localResolver.Item2;
        private List<int> _collidersIDs = new();
        private Dictionary<int, Collider> _collidersList = new();
        private Dictionary<int, int> _collidersHostIdentifierList = new(); // one block can have many colliders, but a collider have only one owner
        private readonly ComplexResolver<IBlocksHost, PlacedBlocksListHandler> _localResolver;

        public bool HaveMultipleColliders => _collidersIDs.Count > 1;
        public int GetColliderID() => _collidersIDs[0];
        public IReadOnlyCollection<int> GetColliderIDs() => _collidersIDs;

        public BlocksColliderModule(Container container) : base(container) 
        {
            _localResolver = new(OnLocalDependenciesResolved, container);
            _localResolver.CheckDependencies();

        }
        private void OnLocalDependenciesResolved()
        {
            BlocksHost.CollidersHost.layer = LayerConstants.GetDefinedLayer(DefinedLayer.Pinplane);

            var blocks = BlocksList.GetPlacedBlocks();
            foreach (var block in blocks)
            {
                var collider = BlocksHost.CollidersHost.AddComponent<BoxCollider>();
                var bounds = block.Properties.ModelSize;
                collider.size = bounds;
                collider.center = 0.5f * bounds.y * Vector3.up;
                AddColliderToList(collider, block.ID);
            }

            BlocksHost.OnBlockPlacedEvent += OnBlockAdded;
        }

        private void OnBlockAdded(PlacedBlock block)
        {
            var collider = BlocksHost.CollidersHost.AddComponent<BoxCollider>();
            var bounds = block.Properties.ModelSize;
            collider.size = bounds;
            collider.center = block.LocalPosition;
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
