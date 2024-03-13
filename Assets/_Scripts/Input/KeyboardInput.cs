using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [DefaultExecutionOrder(-2)]
    public sealed class KeyboardInput : MonoBehaviour 
    {
        private bool _isActive = false;
        private InputController _inputController;
        private float _previousHorizontal = 0f, _previousVertical = 0f;
        private BitArray _controlsMask = new BitArray((int)ControlButtonID.Total, false);

        private void Start()
        {
            ServiceLocatorObject.GetWhenLinkReady<InputController>(Setup);
        }
        private void Setup(InputController inputController)
        {
            _inputController = inputController;
            _isActive = true;
        }
        public void Update()
        {
            if (_isActive)
            {
                /*
                float horizontal = Input.GetAxis("Horizontal");
                if (horizontal != _previousHorizontal)
                {
                    CheckControl(ControlButtonID.MoveLeft, horizontal < 0f);
                    CheckControl(ControlButtonID.MoveRight, horizontal > 0f);
                    _previousHorizontal = horizontal;
                }
                float vertical = Input.GetAxis("Vertical");
                if (vertical != _previousVertical)
                {
                    CheckControl(ControlButtonID.MoveForward, vertical > 0f);
                    CheckControl(ControlButtonID.MoveBack, vertical < 0f);
                    _previousVertical = vertical;
                }
                */
                CheckControl(ControlButtonID.Jump, Input.GetKey(KeyCode.Space));

                CheckControl(ControlButtonID.PlaceBlockButton, Input.GetMouseButtonDown(0));
                CheckControl(ControlButtonID.RightClick, Input.GetMouseButtonDown(1));

                CheckControl(ControlButtonID.RotateCameraLeft, Input.GetKey(KeyCode.Q));
                CheckControl(ControlButtonID.RotateCameraRight, Input.GetKey(KeyCode.E));
            }

            void CheckControl(ControlButtonID id, bool value)
            {
                bool buttonPressed = _controlsMask[(int)id];
                if (buttonPressed != value)
                {
                    if (buttonPressed) _inputController.OnButtonUp(id);
                    else _inputController.OnButtonDown(id);
                    _controlsMask[(int)id] = value;
                }                
            }
        }
    }
}
