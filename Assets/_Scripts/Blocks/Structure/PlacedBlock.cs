using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class PlacedBlock : VirtualBlock // must be class, not struct
    {        
        public readonly int ID;
        public PlacedBlock(int id, Vector3 localPos, PlacingBlockInfo info) : base(localPos,info)
        {
            this.ID = id;
        }

    }
}
