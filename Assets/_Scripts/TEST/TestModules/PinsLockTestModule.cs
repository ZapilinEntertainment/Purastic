using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinsLockTestModule : MonoBehaviour
	{
		[SerializeField] private bool _redraw = false;
        [SerializeField] private float _rotation = 0f;
        [SerializeField] private Vector2Int _lockAddress;
		[SerializeField] private Vector2 _sizeInUnits = Vector2.one;		
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
			const float sz = GameConstants.BLOCK_SIZE;

			Vector2 dir = -0.5f * sz * Vector2.one;
			var rect = new AngledRectangle(
					cutPlanePos + dir,
					_sizeInUnits * GameConstants.BLOCK_SIZE,
					PlaneOrths.Default.RotateOrths(_rotation)
					);

            _basePlate.LockPlateZone(
				rect, 
				out _lockedPins);;
            Debug.Log(rect);
        }
		
    }
}
