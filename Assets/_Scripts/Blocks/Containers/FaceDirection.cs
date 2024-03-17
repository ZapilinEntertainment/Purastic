using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	
	public enum FaceDirection : byte
	{
		Undefined, Forward, Right, Back, Left, Up, Down, Custom
	}
	public static class FaceDirectionExtension
	{
        public static FaceDirection RotateRight(this FaceDirection dir)
        {
            if (dir > FaceDirection.Undefined && dir < FaceDirection.Up)
            {
                if (dir == FaceDirection.Left) return FaceDirection.Forward;
                else return dir+1;
            }
            else return dir;
        }
        public static FaceDirection RotateLeft(this FaceDirection dir)
        {
            if (dir > FaceDirection.Undefined && dir < FaceDirection.Up)
            {
                if (dir == FaceDirection.Forward) return FaceDirection.Left;
                else return dir-1;
            }
            else return dir;
        }
        public static Vector3 ToNormal(this FaceDirection direction)
        {
            switch (direction)
            {
                case FaceDirection.Forward: return Vector3.forward;
                case FaceDirection.Right: return Vector3.right;
                case FaceDirection.Back: return Vector3.back;
                case FaceDirection.Left: return Vector3.left;
                case FaceDirection.Up: return Vector3.up;
                case FaceDirection.Down: return Vector3.down;
                default: return Vector3.zero;
            }
        }
		public static Quaternion ToPlaneRotation(this FaceDirection direction)
		{
            Vector3 look, up;
            switch (direction)
            {
                case FaceDirection.Right:
                    {
                        look = Vector3.right;
                        up = Vector3.up;
                        break;
                    }
                case FaceDirection.Back:
                    {
                        look = Vector3.back;
                        up = Vector3.up;
                        break;
                    }
                case FaceDirection.Left:
                    {
                        look = Vector3.left;
                        up = Vector3.up;
                        break;
                    }
                case FaceDirection.Up:
                    {
                        look = Vector3.up;
                        up = Vector3.back;
                        break;
                    }
                case FaceDirection.Down:
                    {
                        look = Vector3.down;
                        up = Vector3.forward;
                        break;
                    }
                default:
                    {
                        look = Vector3.forward;
                        up = Vector3.up;
                        break;
                    }
            }
            return Quaternion.LookRotation(look, up);
        }
        
    }
}
