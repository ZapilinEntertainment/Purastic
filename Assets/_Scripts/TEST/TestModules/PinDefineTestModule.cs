using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinDefineTestModule : MonoBehaviour
	{
		private bool _isReady = false, _pinFound = false, _propertiesSet = false;
		private BlockProperties f_properties;
		private FoundedFitElementPosition _fitPosition;

		protected bool IsReady => _isReady;
        protected bool PinFound => _pinFound;
		virtual protected BlockPreset BlockPreset => BlockPreset.StandartBrick_1x1;

		protected BlockProperties Properties
		{
			get
			{
				if (!_propertiesSet)
				{
					f_properties = BlockPresetsDepot.GetProperty(BlockPreset, new BlockMaterial(VisualMaterialType.Hologramm, BlockColor.DefaultWhite));
					_propertiesSet = true;
				}
				return f_properties;
            }
		}
		protected FoundedFitElementPosition PositionInfo => _fitPosition;

        protected PlacingBlockModelHandler Marker { get; private set; }
		private BlockCastModule _castModule;			

		private async void Start()
		{			

            _castModule = new BlockCastModule();
			Marker = new();
			while (!(_castModule.IsReady && Marker.IsReady )) await Awaitable.FixedUpdateAsync();
            var modelCreateService = await ServiceLocatorObject.GetWhenLinkReady<BlockCreateService>();
            Marker.ReplaceModel( await modelCreateService.CreateBlockModel(Properties));
            PositionMarker(Vector3.zero);
			_isReady = true;

			AfterStart();
		}
		virtual protected void AfterStart() { }
		

		virtual protected void FixedUpdate()
		{
			if (_isReady)
			{
                if (_castModule.Cast(out _fitPosition, out var hit))
				{
					PositionMarker(_fitPosition.WorldPoint.Position);
					if (!_pinFound)
					{
						Marker.SetModelStatus(_fitPosition.PositionIsObstructed ? BlockPositionStatus.Obstructed : BlockPositionStatus.CanBePlaced);
						_pinFound = true;
					}
				}
				else
				{
					PositionMarker(hit.point);
                    if (_pinFound)
                    {
						Marker.SetModelStatus(BlockPositionStatus.CannotBePlaced);
                        _pinFound = false;
                    }
                }
            }
		}
		virtual protected void PositionMarker(Vector3 groundPos)
        {
            Vector3 pos = groundPos + 0.5f * Properties.ModelSize.y * Vector3.up;
            Marker.Model.SetPoint(pos, _fitPosition.WorldPoint.Rotation);
        }

        virtual protected void OnDrawGizmos()
        {
            if (PinFound) Gizmos.DrawSphere(PositionInfo.WorldPoint.Position, 0.5f);
        }
    }
}
