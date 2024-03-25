using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockPositionHighlightTestModule : PinDefineTestModule
	{
		private bool _isReady = false, _propertiesSet = false;
		private BlockProperties f_properties;
		private FoundedFitElementPosition _fitPosition;

		override protected bool IsReady => base.IsReady & _isReady;
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

        protected PlacingBlockModelHandler Marker { get; private set; }		

		private async void Start()
		{			
			Marker = new();
			while (! Marker.IsReady) await Awaitable.FixedUpdateAsync();
            var modelCreateService = await ServiceLocatorObject.GetWhenLinkReady<BlockCreateService>();
            Marker.ReplaceModel( await modelCreateService.CreateBlockModel(Properties));
            PositionMarker(Vector3.zero);
			_isReady = true;

			AfterStart();
		}
		virtual protected void AfterStart() { }

        protected override void OnFixedUpdate(RaycastHit hit)
        {
            if (PinFound) PositionMarker(_fitPosition.WorldPoint.Position);
			else
			{
                PositionMarker(hit.point);
            }
        }
        protected override void OnPinFound(RaycastHit rh)
        {
			base.OnPinFound(rh);
            Marker.SetModelStatus(_fitPosition.PositionIsObstructed ? BlockPositionStatus.Obstructed : BlockPositionStatus.CanBePlaced);
        }
        protected override void OnPinLost()
        {
			base.OnPinLost();
            Marker.SetModelStatus(BlockPositionStatus.CannotBePlaced);
        }
        virtual protected void PositionMarker(Vector3 groundPos)
        {
            Vector3 pos = groundPos + 0.5f * Properties.ModelSize.y * Vector3.up;
            Marker.Model.SetPoint(pos, _fitPosition.WorldPoint.Rotation);
        }
    }
}
