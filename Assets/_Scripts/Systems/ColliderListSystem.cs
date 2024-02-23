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

		private LinkedColliderOwnersList<PlayerController> _playerColliders = new();

		public void AddPlayerColliders(PlayerController player, IColliderOwner collider) => _playerColliders.AddOwner(player, collider);
		public void RemovePlayerCollider(IColliderOwner collider) => _playerColliders.RemoveOwner(collider);
		public bool TryDefineAsPlayer(int id, out PlayerController player) => _playerColliders.TryGetOwner(id, out player);
		
	}
}
