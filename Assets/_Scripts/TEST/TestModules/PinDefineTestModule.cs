using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinDefineTestModule : MonoBehaviour
	{
		private bool _isReady = false, _pinFound = false;
		private FoundedFitElementPosition _fitPosition;
        protected bool PinFound => _pinFound;
		virtual protected BlockPreset BlockPreset => BlockPreset.StandartBrick_1x1;

		protected BlockProperties _properties;
		protected FoundedFitElementPosition FitPosition => _fitPosition;

        protected PlacingBlockModelHandler Marker { get; private set; }
		private BlockCastModule _castModule;			

		private async void Start()
		{			
			_properties = BlockPresetsDepot.GetProperty(BlockPreset, new BlockMaterial(VisualMaterialType.Hologramm, BlockColor.DefaultWhite)); 

            _castModule = new BlockCastModule();
			Marker = new();
			while (!(_castModule.IsReady && Marker.IsReady )) await Awaitable.FixedUpdateAsync();
            var modelCreateService = await ServiceLocatorObject.GetWhenLinkReady<BlockCreateService>();
            Marker.ReplaceModel( await modelCreateService.CreateBlockModel(_properties));
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
            Vector3 pos = groundPos + 0.5f * _properties.ModelSize.y * Vector3.up;
            Marker.Model.SetPoint(pos, _fitPosition.WorldPoint.Rotation);
        }

        virtual protected void OnDrawGizmos()
        {
            if (PinFound) Gizmos.DrawSphere(FitPosition.WorldPoint.Position, 0.5f);
        }
    }
}
