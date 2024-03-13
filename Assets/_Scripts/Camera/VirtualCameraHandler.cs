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
        private float _modifiedFovValue = 0f, _defaultFov = 60f, _modifiedOffsetValue;        
        private Vector3 _prevPoint;
        private PointViewSettings _viewSettings;
        private Transform _cameraTransform;
        private Transform _targetPoint, _viewPoint;
        private Camera _camera;
        protected bool _modifyOffsetFlag = false;
        protected Transform ViewPoint => _viewPoint;
        virtual protected Vector3 DefaultOffset => Vector3.zero;


        private async void Awake()
        {
            _viewPoint = new GameObject(gameObject.name + "_viewPoint").transform;
            _viewPoint.localPosition = Vector3.zero;
            //_defaultOffset = _followCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>().m_FollowOffset;
            _defaultFov = _followCamera.m_Lens.FieldOfView;
            //_transposer = _followCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();

            _followCamera.Follow = _viewPoint;
            _followCamera.LookAt = _viewPoint;

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
            _viewPoint.position = _targetPoint.position;
        }

        private void Update()
        {
            if (_targetPoint == null || !_isActive) return;

            _viewPoint.position = _targetPoint.position;
            OnUpdateStart();

            Vector3 currentPosition = _targetPoint.position;
            float speed = Vector3.Distance(_prevPoint, currentPosition), t = Time.deltaTime;
            float modifyTarget = 0f;
            if (speed > 0.1f)
            {
                modifyTarget = Mathf.Clamp01((speed / t) / _cameraSettings.MaxCameraSpeed);
            }
            if (modifyTarget != _modifiedFovValue)
            {
                _modifiedFovValue = Mathf.MoveTowards(_modifiedFovValue, modifyTarget, _cameraSettings.FovChangeSpeed * t);
                _followCamera.m_Lens.FieldOfView = Mathf.Lerp(_defaultFov, _cameraSettings.MaxFov, _modifiedFovValue);

            }
            if (modifyTarget != _modifiedOffsetValue)
            {
                _modifiedOffsetValue = Mathf.MoveTowards(_modifiedOffsetValue, modifyTarget, _cameraSettings.OffsetChangeSpeed * t);
                if (_modifyOffset) _modifyOffsetFlag = true;
                //+ _modifiedOffsetValue * _cameraSettings.MaxOffsetZ * Vector3.ProjectOnPlane(_targetPoint.forward, Vector3.up).normalized
            }
            if (_modifyOffsetFlag)
            {
                _viewPoint.localPosition = (DefaultOffset * _viewSettings.HeightViewCf + _modifiedOffsetValue * _cameraSettings.MaxOffsetY * _viewSettings.HeightSpeedOffsetCf * Vector3.up );
                _modifyOffsetFlag = false;
            }

            _prevPoint = currentPosition;            
        }
        virtual protected void OnUpdateStart() { }

        public Vector3 WorldToScreenPoint(Vector3 worldPoint) => _camera.WorldToScreenPoint(worldPoint);
        public Vector3 CameraToWorldDirection(Vector2 dir) => _cameraTransform.rotation * dir;
        public bool TryRaycast(Vector2 screenPos, out RaycastHit hitPoint, int castMask = -1)
        {
            var ray = _camera.ScreenPointToRay(screenPos);
            if (castMask == -1)
            {
                return Physics.Raycast(ray, maxDistance: GameConstants.MAX_POINT_CAST_DISTANCE, hitInfo: out hitPoint) ;                
            }
            else
            {
                return Physics.Raycast(ray, maxDistance: GameConstants.MAX_POINT_CAST_DISTANCE, hitInfo: out hitPoint, layerMask: castMask);
            }
        }
    }
}
