using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinsLockTestModule : MonoBehaviour
	{
		[SerializeField] private bool _redraw = false;
		[SerializeField] private Vector2Int _lockAddress, _size = Vector2Int.one;
		[SerializeField] private RotationStep _rotationStep;
		[SerializeField] private int _rotationValue = 0;
		[SerializeField] private Baseplate _basePlate;
		private IReadOnlyCollection<ConnectingPin> _lockedPins = null;

        private async void Start()
        {
			while (!_basePlate.InitStatusModule.IsInitialized) await Awaitable.FixedUpdateAsync();
			Redraw();
        }
        private void Update()
        {
            if (_redraw)
			{
				_redraw = false;
				Redraw();
			}
        }
		private void Redraw()
		{
			if (_lockedPins != null) _basePlate.UnlockPlateZone(_lockedPins);

			var plane = _basePlate.GetPlatePlane();
			Vector2 cutPlanePos = plane.PlaneAddressToCutPlanePos(new FitElementPlaneAddress(_lockAddress.x, _lockAddress.y));
			var rotation = new Rotation2D(_rotationStep, _rotationValue);
			cutPlanePos = cutPlanePos + rotation * (-0.5f * Vector2.one);

            _basePlate.LockPlateZone(
				new AngledRectangle(
					cutPlanePos.x, cutPlanePos.y, GameConstants.BLOCK_SIZE * _size.x, GameConstants.BLOCK_SIZE * _size.y,
					rotation
					),
				out _lockedPins
				);
		}
    }
}
