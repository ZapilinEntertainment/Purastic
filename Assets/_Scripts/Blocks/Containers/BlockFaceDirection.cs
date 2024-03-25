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
        public Quaternion Rotation => Direction.ToPlaneRotation();

        public BlockFaceDirection(FaceDirection direction)
        {
            this.Direction = direction;
            CustomValue = 0;
        }
        public BlockFaceDirection (Vector3 normal)
        {
            float verticalDot = Utilities.TrimFloat(Vector3.Dot(normal, Vector3.up));
            if (verticalDot == 1f) Direction = FaceDirection.Up;
            else
            {
                if (verticalDot == -1f) Direction = FaceDirection.Down;
                else
                {
                    if (verticalDot == 0f)
                    {
                        float horizontalDot = Utilities.TrimFloat( Vector3.Dot(normal, Vector3.forward));
                        if (horizontalDot == 0f)
                        {
                            float sideDot = Vector3.Dot(normal, Vector3.right);
                            if (sideDot == 1f) Direction = FaceDirection.Right;
                            else
                            {
                                if (sideDot == -1f) Direction = FaceDirection.Left;
                                else Direction = FaceDirection.Custom;
                            }
                        }
                        else
                        {
                            if (horizontalDot == 1f) Direction = FaceDirection.Forward;
                            else
                            {
                                if (horizontalDot == -1f) Direction = FaceDirection.Back;
                                else Direction = FaceDirection.Custom;
                            }
                        }
                    }
                    else
                    {
                        Direction = FaceDirection.Custom;
                    }
                }
            }
            CustomValue = 0;
        }

        public BlockFaceDirection Mirror()
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
        public BlockFaceDirection Rotate(Quaternion rotation) => new BlockFaceDirection((Rotation * rotation) * Vector3.forward);
        public Vector2 InverseVector(Vector3 direction) => new FaceOrths(Direction.ToPlaneRotation()).InverseVector(direction);
        public Vector3 TransformVector(Vector2 facePoint) => new FaceOrths(this).TransformVector(facePoint);

        public Vector3 GetNormalizedZeroPoint()
        {
            //bottom-left corner of a face
            switch (Direction)
            {
                case FaceDirection.Forward: return new Vector3(-1f, -1f, 1f);
                case FaceDirection.Right: return new Vector3(1f, -1f, 1f);
                case FaceDirection.Back: return new Vector3(1f, -1f, -1f);
                case FaceDirection.Left: return new Vector3(-1f, -1f, -1f);
                case FaceDirection.Up: return new Vector3(-1f, 1f, 1f);
                case FaceDirection.Down: return new Vector3(-1f, -1f, -1f);
                default: return Vector3.zero;
            }
        }
        public Quaternion ToRotation() => Direction.ToPlaneRotation();

        // default normal for face with rotation equals to Quaternion.identity
        // right = (1,0,0), up = (0,1,0) , forward = (0,0,1)
        public static Vector3 DefaultFaceRotationNormal => Vector3.forward; 
        public static BlockFaceDirection Up => new(FaceDirection.Up);
        public static BlockFaceDirection Down => new(FaceDirection.Down);
        public static BlockFaceDirection Left => new(FaceDirection.Left);
        public static BlockFaceDirection Right => new(FaceDirection.Right);
        public static BlockFaceDirection Forward => new(FaceDirection.Forward);
        public static BlockFaceDirection Back => new(FaceDirection.Back);
    }
}
