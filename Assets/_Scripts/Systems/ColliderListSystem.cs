using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {
	public sealed class ColliderListSystem
	{
		private class ColliderOwnersList<T> where T : IColliderOwner
		{
			private Dictionary<int, T> _list = new Dictionary<int, T>();
			public void AddOwner(T owner)
			{
				if (owner.HaveMultipleColliders)
				{
					var ids = owner.GetColliderIDs();
					foreach (var id in ids ) { _list.Add(id, owner); }
				}
				else
				{
					_list.Add(owner.GetColliderID(), owner);
				}
			}
			public void OnOwnerChanged(T owner)
			{
                if (owner.HaveMultipleColliders)
                {
                    var ids = owner.GetColliderIDs();
                    foreach (var id in ids) { _list.TryAdd(id, owner); }
                }
                else
                {
                    _list.TryAdd(owner.GetColliderID(), owner);
                }
            }
			public void RemoveOwner(T owner)
			{
				if (owner.HaveMultipleColliders)
				{
                    var ids = owner.GetColliderIDs();
                    foreach (var id in ids) { _list.Remove(id); }
                }
				else
				{
					_list.Remove(owner.GetColliderID());
				}
			}
			public bool TryGetOwner(int id, out T owner)
			{
				return _list.TryGetValue(id, out owner);
			}
		}
		private class LinkedColliderOwnersList<T>
		{
            private Dictionary<int, T> _list = new Dictionary<int, T>();
            public void AddOwner(T host, IColliderOwner owner)
            {
                if (owner.HaveMultipleColliders)
                {
                    var ids = owner.GetColliderIDs();
                    foreach (var id in ids) { _list.Add(id, host); }
                }
                else
                {
                    _list.Add(owner.GetColliderID(), host);
                }
            }
            public void OnOwnerChanged(T host, IColliderOwner collider)
            {
                if (collider.HaveMultipleColliders)
                {
                    var ids = collider.GetColliderIDs();
                    foreach (var id in ids) { _list.TryAdd(id, host); }
                }
                else
                {
                    _list.TryAdd(collider.GetColliderID(), host);
                }
            }
            public void RemoveOwner(IColliderOwner owner)
            {
                if (owner.HaveMultipleColliders)
                {
                    var ids = owner.GetColliderIDs();
                    foreach (var id in ids) { _list.Remove(id); }
                }
                else
                {
                    _list.Remove(owner.GetColliderID());
                }
            }
            public bool TryGetOwner(int id, out T owner)
            {
                return _list.TryGetValue(id, out owner);
            }
        }
        
        private class BlockpartsList
        {
            private class BlockpartsColliderHandler
            {
                private readonly BlockpartsList _host;
                public IReadOnlyCollection<int> CollidersIDs;
                public readonly IBlocksHost BlocksHost;
                private Dictionary<int, BlockpartsColliderHandler> CollidersList => _host._collidersList;

                public BlockpartsColliderHandler(BlockpartsList list, IBlocksHost host)
                {
                    _host = list;
                    BlocksHost = host;
                    CollidersIDs = BlocksHost.GetColliderIDs();
                    foreach (int id in CollidersIDs) CollidersList.Add(id, this);
                    BlocksHost.OnBlockPlacedEvent +=OnBlockAdded;
                }
                private void OnBlockAdded(PlacedBlock block) => Update();
                public void Update()
                {
                    var newIdsList = new List<int> (BlocksHost.GetColliderIDs());
                    foreach (int id in CollidersIDs)
                    {
                        if (!newIdsList.Contains(id)) CollidersList.Remove(id);
                    }
                    foreach (int id in newIdsList)
                    {
                        CollidersList.TryAdd(id, this);
                    }
                    CollidersIDs = newIdsList;
                    //Debug.Log(CollidersIDs.Count);
                }
                public void Clear()
                {
                    foreach (int id in CollidersIDs)
                    {
                        CollidersList.Remove(id);
                    }
                    if (BlocksHost != null) BlocksHost.OnBlockPlacedEvent -= OnBlockAdded;
                }
            }
            private Dictionary<int, BlockpartsColliderHandler> _collidersList = new(), _hostsList = new();

            public void AddBlockpartsCollider(IBlocksHost host)
            {
                var handler = new BlockpartsColliderHandler(this, host);
                _hostsList.Add(host.ID, handler);
            }
            public void UpdateCollider(IBlocksHost host)
            {
                if (_hostsList.TryGetValue(host.ID, out var handler))
                {
                    handler.Update();
                }
                else
                {
                    AddBlockpartsCollider(host);
                }
            }
            public void RemoveBlockpartsCollider(IBlocksHost host)
            {
                if (_hostsList.TryGetValue(host.ID, out var collidersList)) {
                    collidersList.Clear();
                    _hostsList.Remove(host.ID);
                }
            }
            public bool TryDefineBlockhost(int id, out IBlocksHost host) {
               
                if (_collidersList.TryGetValue(id, out var collider))
                {
                    host = collider.BlocksHost; return true;
                }
                else
                {
                    host = null;
                    return false;
                }
            }
        }


        private BlockpartsList _blockPartsList = new();
		private LinkedColliderOwnersList<PlayerController> _playerColliders = new();

		public void AddPlayerColliders(PlayerController player, IColliderOwner collider) => _playerColliders.AddOwner(player, collider);
		public void RemovePlayerCollider(IColliderOwner collider) => _playerColliders.RemoveOwner(collider);
		public bool TryDefineAsPlayer(int id, out PlayerController player) => _playerColliders.TryGetOwner(id, out player);

       
		public void AddBlockhost(IBlocksHost host) => _blockPartsList.AddBlockpartsCollider(host);
        public void UpdateBlockhost(IBlocksHost host) => _blockPartsList.UpdateCollider(host);
        public void RemoveBlockhost(IBlocksHost host) => _blockPartsList.RemoveBlockpartsCollider(host);
        public bool TryDefineBlockhost(int id, out IBlocksHost host) => _blockPartsList.TryDefineBlockhost(id, out host);
	}
}
