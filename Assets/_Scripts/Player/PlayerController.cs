using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;
using System;

namespace ZE.Purastic {
    public sealed class PlayerController : NetworkBehaviour, ICollector
    {
        private bool _isOnOwnersDevice = false;
        private NetworkVariable<PlayerSyncModule> _playerSyncModule = new(writePerm:NetworkVariableWritePermission.Owner);
        private IInputController _inputController;        
        private CharacterCreateService _characterCreateService;        
        private NetworkVariable<Vector3> _someVector = new(writePerm: NetworkVariableWritePermission.Owner);
        public PlayableCharacter ActiveCharacter { get; private set; }
        public Action<PlayableCharacter> OnCharacterChangedEvent;

        private async void Start()
        {
            _isOnOwnersDevice = IsOwner;

            if (_isOnOwnersDevice)
            {
                var container = ServiceLocatorObject.Instance.Container;
                container.RegisterInstance(this);
                var inputController = new InputController(container.GetLink<CameraController>());
                container.RegisterInstance(inputController);
                _inputController = inputController;
            }
            else
            {
                _inputController = new InputControllerPlaceholder();
            }

            _characterCreateService = await ServiceLocatorObject.GetWhenLinkReady<CharacterCreateService>();
            var character = await _characterCreateService.CreateCharacter();
            ChangeCharacter(character);
        }
        private void ChangeCharacter(PlayableCharacter character)
        {
            ActiveCharacter = character;
            ActiveCharacter.transform.parent = transform;
            ActiveCharacter.transform.position = Vector3.one;
            if (_inputController != null) ActiveCharacter.AssignController(_inputController, _isOnOwnersDevice);

            OnCharacterChangedEvent?.Invoke(character);

            if (_isOnOwnersDevice) ServiceLocatorObject.Get<SignalBus>().FireSignal(new CameraViewPointChangedSignal(ActiveCharacter.GetViewPointInfo()));
        }

        private void Update()
        {
            if (IsOwner)
            {
                _playerSyncModule.Value = new PlayerSyncModule() { InputSync = _inputController.GetSync(), CharacterSync = ActiveCharacter.GetSync() };
            }
            else
            {
                _inputController.Sync(_playerSyncModule.Value.InputSync);
                ActiveCharacter.Sync(_playerSyncModule.Value.CharacterSync);

                Vector3 vel = Vector3.zero;
                float interpolationTime = 0.1f;
             //   transform.position = Vector3.SmoothDamp(transform.position, _someVector.Value, ref vel, interpolationTime);
                float targetAngle = 0f, angleVel = 0f;
              //  transform.rotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(transform.rotation.y, targetAngle, ref angleVel, interpolationTime),0f);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public bool TryCollect(ICollectable collectable)
        {
            if (_isOnOwnersDevice) return false;
            if (collectable.IsConsumable)
            {
                // todo
                return true;
            }
            else
            {
               return ActiveCharacter.HandsModule.TryGetInHand(collectable);
            }
        }

        private struct PlayerNetworkData : INetworkSerializable
        {
            private float a, b, c;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref a);
                serializer.SerializeValue(ref b);
                serializer.SerializeValue(ref c);
            }
        }
    }
}
