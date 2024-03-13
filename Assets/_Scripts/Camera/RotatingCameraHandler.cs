using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class RotatingCameraHandler : VirtualCameraHandler
	{
		[SerializeField] private float _rotationSpeed = 30f, _rotationAcceleration = 0.1f;
		[SerializeField] private AnimationCurve _accelerationCurve;
		private float _rotationTargetAbs = 0f, _rotationValueAbs = 0f, _rotationSign = 1f;
		private const float ROTATE_RIGHT_KEY = 1f, ROTATE_LEFT_KEY = -1f;
        protected override Vector3 DefaultOffset => base.DefaultOffset;

        private void Start()
		{
			ServiceLocatorObject.GetWhenLinkReady<InputController>(OnInputControllerSet);
		}
		private void OnInputControllerSet(InputController input)
		{
			input.SubscribeToKeyEvents(ControlButtonID.RotateCameraRight, () => OnRotationButtonPressed(ROTATE_RIGHT_KEY), () => ReleaseRotationButton(ROTATE_RIGHT_KEY));
            input.SubscribeToKeyEvents(ControlButtonID.RotateCameraLeft, () => OnRotationButtonPressed(ROTATE_LEFT_KEY), () => ReleaseRotationButton(ROTATE_LEFT_KEY));
        }
        private void OnRotationButtonPressed(float rotationKey)
        {
			float newSign = Mathf.Sign(rotationKey);
			if (_rotationSign != newSign)
			{
				_rotationValueAbs = 0f;
				_rotationSign = newSign;
			}
            _rotationTargetAbs = rotationKey * newSign; // abs
        }
        private void ReleaseRotationButton(float rotationValue)
		{
			if (_rotationSign == Mathf.Sign(rotationValue))
			{
				_rotationTargetAbs = _rotationValueAbs = 0f;
				_rotationSign = 0f;
			}
		}
		

        protected override void OnUpdateStart()
        {
            if (_rotationTargetAbs != _rotationValueAbs)
			{
				_rotationValueAbs = Mathf.MoveTowards(_rotationValueAbs, _rotationTargetAbs, Time.deltaTime * _rotationAcceleration);
			}
			if (_rotationValueAbs != 0f)
			{
				ViewPoint.localRotation = ViewPoint.rotation * Quaternion.AngleAxis(_rotationSpeed * _accelerationCurve.Evaluate(_rotationValueAbs) * _rotationSign * Time.deltaTime, Vector3.up) ;
			}
        }
    }
}
