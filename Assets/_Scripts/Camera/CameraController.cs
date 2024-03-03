using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class CameraController : MonoBehaviour
	{
		[SerializeField] private int _activeCameraVariant = 0;
        [SerializeField] private Camera _camera;
		[SerializeField] private VirtualCameraHandler[] _cameraVariants;
		private VirtualCameraHandler ActiveCameraHandler => _cameraVariants[_activeCameraVariant];
		public Camera GetCamera() => _camera;
		public new Transform transform => ActiveCameraHandler.transform;

        private void Awake()
        {
			if (ServiceLocatorObject.TryGet<PlayerController>(out var player) && player.ActiveCharacter != null)
			{
				SetTrackPoint(player.ActiveCharacter.GetViewPointInfo());
			}
			ServiceLocatorObject.Get<SignalBus>().SubscribeToSignal<CameraViewPointChangedSignal>(SetTrackPoint);

            for(int i = 0; i < _cameraVariants.Length; i++)
			{
				_cameraVariants[i].gameObject.SetActive(i == _activeCameraVariant);
			}
        }

		private void SetTrackPoint(CameraViewPointChangedSignal signal) => SetTrackPoint(signal.ViewPointInfo);
        private void SetTrackPoint(ViewPointInfo args)
		{
            ActiveCameraHandler.SetTrackPoint(args);
		}       

		public Vector3 WorldToScreenPoint(Vector3 worldPos) => ActiveCameraHandler.WorldToScreenPoint(worldPos);
		public Vector3 CameraToWorldDirection(Vector2 dir) => ActiveCameraHandler.CameraToWorldDirection(dir);
		public bool TryRaycast(Vector2 screenPos, out RaycastHit raycastHit, int castMask = -1) => ActiveCameraHandler.TryRaycast(screenPos,out raycastHit, castMask);

    }
}
