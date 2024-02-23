using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
	public sealed class InputController : IInputController
	{
        private class ControlsMask
        {
            private BitArray _controlsMask = new BitArray((int)ControlButtonID.Total, false);
            public bool this[ControlButtonID id]
            {
                get => _controlsMask[(int)id];
                set => _controlsMask[(int)id] = value;
            }
        }

        private bool _controllableObjectSet = false;
        private bool AreControlsLocked => _controlsLocker.IsLocked;
        private Vector2 _moveVector = Vector3.zero;
        private IPlayerControllable _controllableObject;
        private Locker _controlsLocker = new Locker();
        private ControlsMask _controlsMask = new ControlsMask();
        private CameraController _cameraController;
        public Vector3 MoveVector
        {
            get
            {
                _moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                var vector =_cameraController.CameraToWorldDirection(_moveVector);
                return new Vector3(vector.x, 0f, vector.z).normalized;
            }
            set { }
        }


        public InputController(CameraController cameraController) {
            _cameraController = cameraController;
        }
        public void SetControlObject(IPlayerControllable controllableObject)
        {
            if (_controllableObjectSet)
            {
                ResetInput();
            }
            _controllableObject = controllableObject;
            _controllableObjectSet = _controllableObject != null;
        }

        private int LockControls()
        {
            ResetInput();
            return _controlsLocker.CreateLock();
        }
        private void OnControlsUnlocked(int id)
        {
            if (AreControlsLocked)
            {
                _controlsLocker.DeleteLock(id);
                Recalculation();
            }
        }

        public void OnButtonDown(ControlButtonID button) {
            _controlsMask[button] = true;
            Recalculation();
        }
        public void OnButtonUp(ControlButtonID button) {
            _controlsMask[button] = false;
            Recalculation();
        }
        private void Recalculation()
        {
            if (AreControlsLocked) return;
            float x = 0f, y = 0f;
            if (_controlsMask[ControlButtonID.MoveLeft]) x = -1f;
            else
            {
                if (_controlsMask[ControlButtonID.MoveRight]) x = 1f;
            }
            if (_controlsMask[ControlButtonID.MoveForward]) y = 1f;
            else
            {
                if (_controlsMask[ControlButtonID.MoveBack]) y = -1f;
            }
            _moveVector = new Vector2(x, y);
            

            if (_controlsMask[ControlButtonID.Jump] && _controllableObjectSet) _controllableObject.Jump(); 
        }
        private void ResetInput()
        {
            _moveVector = Vector2.zero;
        }

        public void Sync(Synchronizer sync) { }
        public Synchronizer GetSync() => new Synchronizer() { IsJumping = _controlsMask[ControlButtonID.Jump], MoveVector = MoveVector };

        public struct Synchronizer : INetworkSerializable
        {
            public bool IsJumping;
            public Vector3 MoveVector;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref IsJumping);
                serializer.SerializeValue(ref MoveVector);
            }
        }
    }
}
