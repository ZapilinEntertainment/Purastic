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
        public override string ToString()
        {
            return $"{Direction}({CustomValue})";
        }
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

        public BlockFaceDirection Inverse()
        {
            switch (Direction)
            {
                case FaceDirection.Forward: return new BlockFaceDirection(FaceDirection.Back);
                case FaceDirection.Right: return new BlockFaceDirection(FaceDirection.Left);
                case FaceDirection.Back: return new BlockFaceDirection(FaceDirection.Forward);
                case FaceDirection.Left: return new BlockFaceDirection(FaceDirection.Right);
                case FaceDirection.Up: return new BlockFaceDirection(FaceDirection.Down);
                case FaceDirection.Down: return new BlockFaceDirection(FaceDirection.Up);
                default: return this;
            }
        }
        public Vector2 InverseVector(Vector3 projectedDir)
        {
            var rotation = Direction.ToPlaneRotation();
            Vector3 right = rotation * Vector3.right, up = rotation * Vector3.up;
            float x = Vector3.Dot(projectedDir, right),
                y = Vector3.Dot(projectedDir, up);
            return new Vector2(x, y);
        }
        public Vector3 TransformVector(Vector2 planePos, IBlocksHost host)
        {
            Vector3 planeZeroPos3d = GetZeroPosition(Vector3.zero, host.ZeroPoint);
            var rotation = ToRotation();
            Vector3 planeRight = rotation * Vector3.right, planeUp = rotation * Vector3.up;
            return planeZeroPos3d + planeRight * planePos.x + planeUp * planePos.y;
        }

        public Vector3 GetZeroPosition(Vector3 modelCenter, Vector3 modelSize) => Direction.GetZeroPos(modelCenter, modelSize);
        public Quaternion ToRotation() => Direction.ToPlaneRotation();
        public Rotation2D ToPlaneRotation() => Rotation2D.NoRotation; // wait for custom
    }
}
