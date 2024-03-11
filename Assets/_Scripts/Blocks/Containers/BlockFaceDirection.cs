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
        public BlockFaceDirection (Vector3 normal)
        {
            float verticalDot = Vector3.Dot(normal, Vector3.up);
            if (verticalDot == 1f) Direction = FaceDirection.Up;
            else
            {
                if (verticalDot == -1f) Direction = FaceDirection.Down;
                else
                {
                    if (verticalDot == 0f)
                    {
                        float horizontalDot = Vector3.Dot(normal, Vector3.forward);
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
        public Vector2 LocalToFaceDirection(Vector3 direction)
        {
            var rotation = Direction.ToPlaneRotation();
            Vector3 right = rotation * Vector3.right, up = rotation * Vector3.up;
            float x = Vector3.Dot(direction, right),
                y = Vector3.Dot(direction, up);
            return new Vector2(x, y);
        }
        public Vector3 TransformPoint(Vector2 facePoint)
        {
            var rotation = ToRotation();
            return rotation * new Vector3(facePoint.x, facePoint.y, 0f);
        }

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
        public Rotation2D GetHorizontalRotation()
        {
            switch (Direction) {
                case FaceDirection.Right: return Rotation2D.SquareRotation(1);
                case FaceDirection.Back: return Rotation2D.SquareRotation(2);
                case FaceDirection.Left: return Rotation2D.SquareRotation(-1);
                default: return Rotation2D.NoRotation;
            }
        }
        public Rotation2D GetVerticalRotation()
        {
            switch (Direction)
            {
                case FaceDirection.Up: return Rotation2D.SquareRotation(1);
                case FaceDirection.Down: return Rotation2D.SquareRotation(-1);
                default: return Rotation2D.NoRotation;
            }
        }
        public PlacedBlockRotation ToBlockRotation() => new PlacedBlockRotation(GetHorizontalRotation(), GetVerticalRotation());

        public static BlockFaceDirection Up => new(FaceDirection.Up);
        public static BlockFaceDirection Down => new(FaceDirection.Down);
        public static BlockFaceDirection Left => new(FaceDirection.Left);
        public static BlockFaceDirection Right => new(FaceDirection.Right);
        public static BlockFaceDirection Forward => new(FaceDirection.Forward);
        public static BlockFaceDirection Back => new(FaceDirection.Back);
    }
}
