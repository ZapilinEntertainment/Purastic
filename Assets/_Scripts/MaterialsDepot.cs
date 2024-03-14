using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class MaterialsDepot
	{
        private VisualMaterialsPack _materialsPack;
		private ColorSettings _colorSettings;
        private Dictionary<BlockMaterial, Material> _materials = new();
		private Dictionary<BlockPositionStatus, Material> _positionStatusMaterials = new();
		

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
		public Material GetPlacingBlockMaterial(BlockPositionStatus status)
		{
			Material material;
			if (!_positionStatusMaterials.TryGetValue(status, out material))
			{
				material = Object.Instantiate(_materialsPack.GetMaterial(VisualMaterialType.Hologramm));
                material.color = _colorSettings.GetPlaceColor(status);
				_positionStatusMaterials.Add(status, material);
            }
			return material;
		}
	}
}
