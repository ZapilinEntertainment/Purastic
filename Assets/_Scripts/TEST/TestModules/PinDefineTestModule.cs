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

        protected BlockModel Marker { get; private set; }
		private BlockCastModule _castModule;
		private Material _availableMaterial, _blockedMaterial;		

		private async void Start()
		{			
			_properties = BlockPresetsDepot.GetProperty(BlockPreset, new BlockMaterial(VisualMaterialType.Hologramm, BlockColor.DefaultWhite)); 

            _castModule = new BlockCastModule();
			var resolver = new ComplexResolver<BlockCreateService, MaterialsDepot>(null);
			resolver.CheckDependencies();

			while (!(_castModule.IsReady && resolver.AllDependenciesCompleted )) await Awaitable.FixedUpdateAsync();
			Marker = await resolver.Item1.CreateBlockModel(_properties);
			_availableMaterial = resolver.Item2.GetPlacingBlockMaterial(true);
			_blockedMaterial = resolver.Item2.GetPlacingBlockMaterial(false);
			Marker.SetDrawMaterial(_blockedMaterial);
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
						Marker.SetDrawMaterial(_availableMaterial);
						_pinFound = true;
					}
				}
				else
				{
					PositionMarker(hit.point);
                    if (_pinFound)
                    {
                        Marker.SetDrawMaterial(_blockedMaterial);
                        _pinFound = false;
                    }
                }
            }
		}
		virtual protected void PositionMarker(Vector3 groundPos)
        {
            Vector3 pos = groundPos + 0.5f * _properties.ModelSize.y * Vector3.up;
            Marker.transform.position = pos;
        }

        private void OnDrawGizmos()
        {
            if (PinFound) Gizmos.DrawSphere(FitPosition.WorldPoint.Position, 0.5f);
        }
    }
}
