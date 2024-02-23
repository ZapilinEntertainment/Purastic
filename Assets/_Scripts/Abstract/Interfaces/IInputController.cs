using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IInputController
	{
        public Vector3 MoveVector { get; set; }
        public void SetControlObject(IPlayerControllable controllableObject);

        public void Sync(InputController.Synchronizer sync);
        public InputController.Synchronizer GetSync();

    }

    public class InputControllerPlaceholder : IInputController
    {
        private IPlayerControllable _controllable;
        public Vector3 MoveVector { get; set; }
        public void SetControlObject(IPlayerControllable playerControllable) { _controllable = playerControllable; }
        public void Sync(InputController.Synchronizer sync)
        {
            MoveVector = sync.MoveVector;
            if (sync.IsJumping) _controllable.Jump();
        }
        public InputController.Synchronizer GetSync() => new();
    }
}
