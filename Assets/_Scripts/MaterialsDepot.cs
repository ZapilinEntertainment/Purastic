using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class MaterialsDepot
	{
        private VisualMaterialsPack _materialsPack;
        private Dictionary<BlockMaterial, Material> _materials = new();
		

		public MaterialsDepot() { 
			ServiceLocatorObject.GetWhenLinkReady((VisualMaterialsPack pack) => _materialsPack = pack);
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
	}
}
