using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class Utilities
	{

        private static System.Random _random = new();
        public static int GenerateInteger() => _random.Next();

        public static AngledRectangle ProjectBlock(ICuttingPlane cuttingPlane, VirtualBlock block)
        {
            var rotation = block.Rotation;
            var localBlockDirection = rotation.TransformDirection(cuttingPlane.Face);

            Vector3 localSize = rotation.Quaternion * block.Properties.ModelSize;
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
            return new AngledRectangle(new Rect(cuttingPlane.LocalToPlanePos(block.LocalPosition), size), Vector2.up, Vector2.right);
        }
    }
}
