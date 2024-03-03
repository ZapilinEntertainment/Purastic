using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/VisualMaterialsPack")]
    public sealed class VisualMaterialsPack : ScriptableObject
    {
        [field: SerializeField] public Material Plastic { get; private set; }
        [field: SerializeField] public Material PlacingPart { get; private set; }

        public Material GetMaterial(VisualMaterialType type)
        {
            if (type == VisualMaterialType.PlacingPart) return PlacingPart;
            else return Plastic;
        }
    }
}
