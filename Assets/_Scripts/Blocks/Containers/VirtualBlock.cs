using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class VirtualBlock // must be class, not struct
    {
        public readonly Vector3 LocalPosition;
        public readonly PlacedBlockRotation Rotation;
        public readonly BlockProperties Properties;

        public VirtualBlock(Vector3 localPos, PlacingBlockInfo info)
        {
            this.LocalPosition = localPos;
            this.Properties = info.Properties;
            this.Rotation = info.Rotation;
        }

        public AngledRectangle GetCutPlaneRectangle(ICuttingPlane cuttingPlane)
        {
            var localBlockDirection = Rotation.TransformDirection(cuttingPlane.Face);

            Vector3 localSize = Rotation.Quaternion * Properties.ModelSize;
            Vector2 size;
            //for non-custom
            switch (localBlockDirection.Direction)
            {                    
                case FaceDirection.Up:
                    {
                        size = new Vector2(localSize.x, localSize.z);
                        break;
                    }
                case FaceDirection.Right:
                    {
                        size = new Vector2(localSize.z, localSize.y);
                        break;
                    }
                case FaceDirection.Back:
                    {
                        size = new Vector2(localSize.x, localSize.y);
                        break;
                    }
                case FaceDirection.Left:
                    {
                        size = new Vector2(localSize.z, localSize.y);
                        break;
                    }
                default:
                    {
                        size = new Vector2(localSize.x, localSize.y);
                        break;
                    }
            }
            return new AngledRectangle(new Rect(cuttingPlane.LocalToPlanePos(LocalPosition), size), Vector2.up, Vector2.right);

            /*
            Vector3 blockPlaneZeroPoint = localBlockDirection.GetZeroPosition(LocalPosition, Properties.ModelSize);

            Vector2 planeProjectedZeroPos = cuttingPlane.LocalToPlanePos(blockPlaneZeroPoint);
            var blockPlaneRotation = localBlockDirection.ToRotation();
            Vector3 projectedUpDirection = Vector3.ProjectOnPlane(blockPlaneRotation * Vector3.up, localBlockDirection.Normal).normalized,
                projectedRightDirection = Vector3.ProjectOnPlane(blockPlaneRotation * Vector3.left, localBlockDirection.Normal).normalized;
            Vector2 projectedSize = localBlockDirection.InverseVector(Vector3.ProjectOnPlane( Rotation.Quaternion * Properties.ModelSize, localBlockDirection.Normal));
            projectedSize = new Vector2(Mathf.Abs(projectedSize.x), Mathf.Abs(projectedSize.y));
            return new AngledRectangle(new Rect(planeProjectedZeroPos, projectedSize), projectedUpDirection, projectedRightDirection);
            */
        }
        public Vector3 TransformPoint(Vector2 inPlanePoint, BlockFaceDirection face)
        {
            Vector3 zeroPos = face.GetZeroPosition(LocalPosition, Properties.ModelSize);
            return zeroPos + face.ToRotation() * (inPlanePoint.x * Vector3.right + inPlanePoint.y * Vector3.up);
        }
        public BlockFaceDirection DefineFaceDirection(Vector3 structureSpaceHitPos)
        {
            Vector3 size = Properties.ModelSize;
            Vector3 localPos = Matrix4x4.TRS(LocalPosition, Rotation.Quaternion, Vector3.one) * structureSpaceHitPos; // local pos in brick coordinates
            FaceDirection direction;
            sbyte Yquadrant = 0;

            if (localPos.y >= size.y) Yquadrant = 1; else if (localPos.y <= 0f) Yquadrant = -1;
            if (Yquadrant == 0)
            {
                sbyte Xquadrant = 0;
                if (localPos.x >= size.x * 0.5f) Xquadrant = 1; else if (localPos.x <= -size.x * 0.5f) Xquadrant = -1;
                if (Xquadrant == 0)
                {
                    sbyte Zquadrant = 0;
                    if (localPos.z >= size.z * 0.5f) Zquadrant = 1; else if (localPos.z <= -size.z * 0.5f) Zquadrant = -1;
                    if (Zquadrant == 1) direction = FaceDirection.Forward;
                    else
                    {
                        if (Zquadrant != 0) direction = FaceDirection.Back;
                        else direction = FaceDirection.Undefined;
                    }
                }
                else
                {
                    if (Xquadrant == 1) direction = FaceDirection.Right; else direction = FaceDirection.Left;
                }
            }
            else
            {
                if (Yquadrant == 1) direction = FaceDirection.Up;
                else direction = FaceDirection.Down;
            }
            return new BlockFaceDirection(direction);
        }
    }
}
