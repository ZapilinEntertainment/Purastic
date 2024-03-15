using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public readonly struct CuttingPlanePosition : System.IEquatable<CuttingPlanePosition>
    {
        public readonly BlockFaceDirection Face;
        public readonly float Coordinate;

        public CuttingPlanePosition(BlockFaceDirection direction, float coordinate)
        {
            Face = direction;
            Coordinate = Utilities.TrimFloat(coordinate);
        }
        public override string ToString() => $"{Face} :{Coordinate}";
        #region equality
        public override bool Equals(object obj) => obj is CuttingPlanePosition other && this.Equals(other);
        public bool Equals(CuttingPlanePosition p) => Face == p.Face && Coordinate == p.Coordinate;
        public override int GetHashCode() => (Face, Coordinate).GetHashCode();
        public static bool operator ==(CuttingPlanePosition lhs, CuttingPlanePosition rhs) => lhs.Equals(rhs);
        public static bool operator !=(CuttingPlanePosition lhs, CuttingPlanePosition rhs) => !(lhs == rhs);
        #endregion
    }
}
