using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class PlacedBlock // must be class, not struct
    {        
        public readonly int ID;
        public readonly Vector3 LocalPosition;
        public readonly PlacedBlockRotation Rotation;
        public readonly BlockProperties Properties;

        public PlacedBlock(int id,Vector3 localPosition,  BlockProperties properties)
        {
            this.ID = id;
            this.LocalPosition = localPosition;
            this.Properties = properties;
        }
        public PlacedBlock(int id, Vector3 localPos, BlockProperties properties, PlacedBlockRotation rotation) : this(id,localPos, properties)
        {
            this.Rotation = rotation;
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
