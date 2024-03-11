using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class PinDefineTestModule : MonoBehaviour
	{
		private bool _isReady = false, _pinFound = false;
		private BlockModel _marker;
		private BlockCastModule _castModule;
		private Material _availableMaterial, _blockedMaterial;

		private async void Start()
		{
            _castModule = new BlockCastModule();
			var resolver = new ComplexResolver<BlockCreateService, MaterialsDepot>(null);
			resolver.CheckDependencies();

			while (!(_castModule.IsReady && resolver.AllDependenciesCompleted )) await Awaitable.FixedUpdateAsync();
			_marker = await resolver.Item1.CreateBlockModel(BlockPresetsDepot.GetProperty(BlockPreset.StandartBrick_1x1, new BlockMaterial(VisualMaterialType.Hologramm, BlockColor.DefaultWhite)));
			_availableMaterial = resolver.Item2.GetPlacingBlockMaterial(true);
			_blockedMaterial = resolver.Item2.GetPlacingBlockMaterial(false);
			_marker.SetDrawMaterial(_blockedMaterial);
			_isReady = true;
		}
		

		private void FixedUpdate()
		{
			if (_isReady)
			{
				if (_castModule.Cast(out FoundedFitElementPosition position, out var hit))
				{
					_marker.transform.position = position.WorldPoint.Position;
					if (!_pinFound)
					{
						_marker.SetDrawMaterial(_availableMaterial);
						_pinFound = true;
					}
				}
				else
				{
					_marker.transform.position = hit.point;
                    if (_pinFound)
                    {
                        _marker.SetDrawMaterial(_blockedMaterial);
                        _pinFound = false;
                    }
                }
            }
		}
	}
}
