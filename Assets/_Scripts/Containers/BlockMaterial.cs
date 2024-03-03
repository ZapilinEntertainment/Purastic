using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
    [Serializable]
    public struct BlockMaterial
    {
        [field: SerializeField] public VisualMaterialType VisualMaterialType{ get; private set; }
        [field: SerializeField] public BlockColor BlockColor { get; private set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(BlockColor.GetHashCode(), VisualMaterialType.GetHashCode());
        }
    }
}
