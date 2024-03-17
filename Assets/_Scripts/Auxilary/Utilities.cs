using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class Utilities
	{

        private static System.Random _random = new();
        public static int GenerateInteger() => _random.Next();
        public static float TrimFloat(float y) => (float)System.Math.Round(y, 5);
        public static Vector2 TrimVector(Vector2 pos) => new Vector2(TrimFloat(pos.x), TrimFloat(pos.y));


        public static CuttingPlanePosition DefineCutPlaneCoordinate(VirtualBlock block, BlockFaceDirection face) => 
            new CuttingPlanePosition(face, DefineCutPlaneCoordinate(block.GetFaceZeroPointInLocalSpace(face), face.Normal));
        public static float DefineCutPlaneCoordinate(Vector3 localPos, Vector3 planeNormal)
        {
            Vector3 projectedPos = Vector3.ProjectOnPlane(localPos, planeNormal);
            return Vector3.Dot(localPos - projectedPos, planeNormal);
        }
        public static AngledRectangle ProjectBlock(BlockFaceDirection face, VirtualBlock block)
        {
            var rotation = block.Rotation;
            var localBlockDirection = face.Rotate(rotation);

            Vector2 localSize = block.Properties.GetProjectionSize(face);
            Vector2 size, zeroPos = Vector2.zero;
            //for non-custom
            switch (localBlockDirection.Direction)
            {
                case FaceDirection.Up:
                    {
                        var orths = new FaceOrths(rotation * face.Rotation);
                        //Debug.Log($"{orths} -> {orths.ToPlaneOrths(face.Normal)}");
                        zeroPos = face.LocalToFaceDirection(block.LocalPosition) - 0.5f * localSize;
                        Debug.Log($"{block.LocalPosition} -> {face.LocalToFaceDirection(block.LocalPosition)}");
                        return new (zeroPos, localSize, orths.ToPlaneOrths(face.Normal));
                    }
                case FaceDirection.Down:
                    {
                       //size = new Vector2(localSize.x, localSize.z);
                       // zeroPos = face.InverseVector(block.TransformNormalizedPoint(-1, 1f, -1f));
                        break;
                    }

                    // todo!
                case FaceDirection.Right:
                    {
                        //vertical rotation = 0
                       // size = new Vector2(localSize.z, localSize.y);
                        var projectedPos = Vector3.ProjectOnPlane(block.TransformNormalizedPoint(1, -1f, 1f), Vector3.up);
                        zeroPos = new Vector2(projectedPos.x, projectedPos.z);

                        // set rect rotation in dependence of vertical rotation
                        break;
                    }
                case FaceDirection.Back:
                    {
                        size = new Vector2(localSize.x, localSize.y);
                        var projectedPos = Vector3.ProjectOnPlane(block.TransformNormalizedPoint(-1, -1f, -1f), Vector3.up);
                        zeroPos = new Vector2(projectedPos.x, projectedPos.z);
                        break;
                    }
                case FaceDirection.Left:
                    {
                       // size = new Vector2(localSize.z, localSize.y);
                        var projectedPos = Vector3.ProjectOnPlane(block.TransformNormalizedPoint(-1, -1f, -1f), Vector3.up);
                        zeroPos = new Vector2(projectedPos.x, projectedPos.z);
                        break;
                    }
                default:
                    {
                        size = new Vector2(localSize.x, localSize.y);
                        break;
                    }
            }
            return new AngledRectangle();
        }
    }
}
