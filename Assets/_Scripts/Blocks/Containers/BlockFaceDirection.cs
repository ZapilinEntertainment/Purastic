using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
    public readonly struct BlockFaceDirection : IEquatable<BlockFaceDirection> 
    {
        public readonly FaceDirection Direction;
        public readonly sbyte CustomValue;
        #region equality
        public override bool Equals(object obj) => obj is BlockFaceDirection other && this.Equals(other);
        public bool Equals(BlockFaceDirection p) => Direction == p.Direction && CustomValue == p.CustomValue;
        public override int GetHashCode() => (Direction, CustomValue).GetHashCode();
        public static bool operator ==(BlockFaceDirection lhs, BlockFaceDirection rhs) => lhs.Equals(rhs);
        public static bool operator !=(BlockFaceDirection lhs, BlockFaceDirection rhs) => !(lhs == rhs);
        #endregion
        public Vector3 Normal
        {
            get
            {
                Vector3 baseDirection = Direction.ToNormal();
                if (Direction != FaceDirection.Custom)
                {
                    return baseDirection;
                }
                else
                {
                    //indev
                    return Vector3.up;
                }
            }
        }

        public BlockFaceDirection(FaceDirection direction)
        {
            this.Direction = direction;
            CustomValue = 0;
        }

        public Vector2 ToPlanePosition(Vector3 projectedDir)
        {
            var rotation = Direction.ToPlaneRotation();
            Vector3 right = rotation * Vector3.right, up = rotation * Vector3.up;
            float x = Vector3.Dot(projectedDir, right),
                y = Vector3.Dot(projectedDir, up);
            return new Vector2(x, y);
        }
        public Vector3 GetZeroPosition(Vector3 modelCenter, Vector3 modelSize) => Direction.GetZeroPos(modelCenter, modelSize);
        public Quaternion ToRotation() => Direction.ToPlaneRotation();
    }
}
