using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class MaterialsDepot
	{
        private VisualMaterialsPack _materialsPack;
		private ColorSettings _colorSettings;
		private Material _placingAvailableMaterial, _placingUnavailableMaterial;
        private Dictionary<BlockMaterial, Material> _materials = new();
		

		public MaterialsDepot() { 
			ServiceLocatorObject.GetWhenLinkReady((VisualMaterialsPack pack) => _materialsPack = pack);
            ServiceLocatorObject.GetWhenLinkReady((ColorSettings pack) => _colorSettings = pack);
        }

		public Material GetVisualMaterial(BlockMaterial material)
		{
			Material graphicMaterial;
			if (!_materials.TryGetValue(material, out graphicMaterial))
			{
				graphicMaterial = _materialsPack.GetMaterial(material.VisualMaterialType);
				if (material.BlockColor != BlockColor.DefaultWhite)
				{
					graphicMaterial = Object.Instantiate(graphicMaterial);
					graphicMaterial.color =material.BlockColor.ToColor32();
				}
				_materials.Add(material, graphicMaterial);
			}
			return graphicMaterial;
		}
		public Material GetPlacingBlockMaterial(bool placingAvailable)
		{
			if (placingAvailable)
			{
				if (_placingAvailableMaterial == null)
				{
					_placingAvailableMaterial = Object.Instantiate(_materialsPack.GetMaterial(VisualMaterialType.Hologramm));
					_placingAvailableMaterial.color = _colorSettings.PlaceBlockColor_Available;
				}
				return _placingAvailableMaterial;
			}
			else
			{
                if (_placingUnavailableMaterial == null)
                {
                    _placingUnavailableMaterial = Object.Instantiate(_materialsPack.GetMaterial(VisualMaterialType.Hologramm));
                    _placingUnavailableMaterial.color = _colorSettings.PlaceBlockColor_Unavailable;
                }
                return _placingUnavailableMaterial;
            }
		}
	}
}
