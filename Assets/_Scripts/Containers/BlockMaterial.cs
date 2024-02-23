using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
    [Serializable]
    public class BlockMaterial
    {
		[field:SerializeField] public float Mass { get; private set; } = 1f;
        [field: SerializeField] public Material VisibleMaterial { get; private set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Mass, VisibleMaterial.GetHashCode());
        }
    }
}
