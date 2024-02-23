using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
    [RequireComponent(typeof(Collider))]
	public class PlayerTrigger : NetworkBehaviour
	{
        
        private int _insideColliderID = -1;
        private ColliderListSystem _collidersList;
        protected float Radius => (_trigger.bounds.max - _trigger.bounds.center).magnitude;
        protected Collider _trigger;
        protected PlayerController _player;
        public bool IsPlayerInside { get; private set; } = false;
        public Action OnPlayerExitEvent;
        public Action<PlayerController> OnPlayerEnterEvent;

        virtual protected void Awake()
        {
            _trigger = GetComponent<Collider>();
            _trigger.isTrigger = true;
            if (IsServer)
            {
                _trigger.gameObject.layer = LayerConstants.GetDefinedLayer(DefinedLayer.Collectable);
                ServiceLocatorObject.GetWhenLinkReady((ColliderListSystem collidersList) => _collidersList = collidersList);
            }
            else
            {
                _trigger.enabled = false;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;
            int id = other.GetInstanceID();
            if (_collidersList.TryDefineAsPlayer(id, out var player))
            {
                _insideColliderID = id;
                _player = player;
                OnPlayerEnter(player);
            }
        }
        virtual protected void OnPlayerEnter(PlayerController player)
        {
            IsPlayerInside = true;
            OnPlayerEnterEvent?.Invoke(_player);
        }
        private void OnTriggerExit(Collider other)
        {
            if (IsPlayerInside && other.GetInstanceID() == _insideColliderID)
            {
                IsPlayerInside = false;
                _insideColliderID = -1;
                OnPlayerExitEvent?.Invoke();
            }
        }
        virtual public void Dispose()
        {
            if (!IsServer) return;
            if (IsPlayerInside) OnPlayerExitEvent?.Invoke();
            Destroy(gameObject);

        }
    }
}
