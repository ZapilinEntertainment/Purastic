using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public  class VirtualCameraHandler : MonoBehaviour
	{
        [SerializeField] private bool _modifyOffset = true, _modifyFov = true;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _followCamera;
        [SerializeField] private CameraSettings _cameraSettings;

        private bool _isActive = false;
        private float _modifiedCameraValue = 0f, _defaultFov = 60f, _modifiedOffsetValue;        
        private Vector3 _prevPoint, _defaultOffset;
        private PointViewSettings _viewSettings;
        private Transform _cameraTransform;
        private Transform _targetPoint;
        private Camera _camera;
        private Cinemachine.CinemachineTransposer _transposer;


        private async void Awake()
        {  
            _defaultFov = _followCamera.m_Lens.FieldOfView;
            _transposer = _followCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
            _defaultOffset = _transposer.m_FollowOffset;

            var cameraController = await ServiceLocatorObject.GetWhenLinkReady<CameraController>();
            _camera = cameraController.GetCamera();
            _cameraTransform = _camera.transform;
            _prevPoint = _cameraTransform.position;
            _isActive = true;
        }

        public void SetTrackPoint(ViewPointInfo args)
        {
            _targetPoint = args.ViewPoint;
            _viewSettings = args.PointViewSettings;
            _followCamera.m_LookAt = _targetPoint;
            _followCamera.m_Follow = _targetPoint;
        }

        private void Update()
        {
            if (_targetPoint == null || _isActive) return;
            Vector3 currentPosition = _targetPoint.position;
            float speed = Vector3.Distance(_prevPoint, currentPosition), t = Time.deltaTime;
            float modifyTarget = 0f;
            if (speed > 0.1f)
            {
                modifyTarget = Mathf.Clamp01((speed / t) / _cameraSettings.MaxCameraSpeed);
            }
            if (modifyTarget != _modifiedCameraValue)
            {
                _modifiedCameraValue = Mathf.MoveTowards(_modifiedCameraValue, modifyTarget, _cameraSettings.FovChangeSpeed * t);
                _followCamera.m_Lens.FieldOfView = Mathf.Lerp(_defaultFov, _cameraSettings.MaxFov, _modifiedCameraValue);

            }
            if (modifyTarget != _modifiedOffsetValue)
            {
                _modifiedOffsetValue = Mathf.MoveTowards(_modifiedOffsetValue, modifyTarget, _cameraSettings.OffsetChangeSpeed * t);
                if (_modifyOffset) _transposer.m_FollowOffset = (_defaultOffset * _viewSettings.HeightViewCf + _modifiedOffsetValue * _cameraSettings.MaxOffsetY * Vector3.up * _viewSettings.HeightSpeedOffsetCf);
                //+ _modifiedOffsetValue * _cameraSettings.MaxOffsetZ * Vector3.ProjectOnPlane(_targetPoint.forward, Vector3.up).normalized
            }


            _prevPoint = currentPosition;
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPoint) => _camera.WorldToScreenPoint(worldPoint);
        public Vector3 CameraToWorldDirection(Vector2 dir) => _cameraTransform.rotation * dir;
    }
}
