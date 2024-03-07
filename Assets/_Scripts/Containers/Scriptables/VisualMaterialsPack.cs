using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/VisualMaterialsPack")]
    public sealed class VisualMaterialsPack : ScriptableObject
    {
        [field: SerializeField] public Material Plastic { get; private set; }
        [field: SerializeField] public Material Hologramm { get; private set; }

        public Material GetMaterial(VisualMaterialType type)
        {
            if (type == VisualMaterialType.Hologramm) return Hologramm;
            else return Plastic;
        }
    }
}
