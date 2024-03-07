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
            switch (direction)
            {
                case FaceDirection.Right: return Quaternion.AngleAxis(90f, Vector3.up);
                case FaceDirection.Back: return Quaternion.AngleAxis(180f, Vector3.up);
                case FaceDirection.Left: return Quaternion.AngleAxis(-90f, Vector3.up);
                case FaceDirection.Up: return Quaternion.AngleAxis(90f, Vector3.left);
                case FaceDirection.Down: return Quaternion.AngleAxis(-90f, Vector3.left);
                default: return Quaternion.identity;
            }
        }
		public static Vector3 GetZeroPos(this FaceDirection direction, Vector3 modelCenter, Vector3 modelSize)
		{            
            // zero pos is bottom-left corner (like ui)
            switch (direction)
            {
                case FaceDirection.Forward: return modelCenter + 0.5f * new Vector3(-modelSize.x, -modelSize.y, modelSize.z);
                case FaceDirection.Right: return modelCenter + 0.5f * new Vector3(modelSize.x, -modelSize.y, modelSize.z);
                case FaceDirection.Back: return modelCenter + 0.5f * new Vector3(modelSize.x, -modelSize.y, -modelSize.z);
                case FaceDirection.Left: return modelCenter + 0.5f * new Vector3(-modelSize.x, -modelSize.y, -modelSize.z);
                case FaceDirection.Up: return modelCenter + 0.5f * new Vector3(-modelSize.x, modelSize.y, modelSize.z);
                case FaceDirection.Down: return modelCenter + 0.5f * new Vector3(-modelSize.x, -modelSize.y, -modelSize.z);
                default: return Vector3.zero;
            }
        }
        
    }
}
