using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PlaneOrths
	{
		public readonly Vector2 Right; 
		public readonly Vector2 Up;
		public readonly Quaternion Quaternion {
			get
			{
				Vector3 right = Right, up= Up;
				return Quaternion.LookRotation(Vector3.Cross(right, up), up);
			}
		}
		public static PlaneOrths Default => new PlaneOrths(Vector2.right, Vector2.up);
        public override string ToString()
        {
			return $"{Right}x{Up}";
        }

        public PlaneOrths(Vector2 right, Vector2 up)
		{
			Right = right;
			Up = up;
		}

		public Vector2 InverseVector(Vector2 dir) => new Vector2(Vector2.Dot(dir, Right), Vector2.Dot(dir, Up));
		public Vector2 TransformVector(Vector2 dir) => dir.x * Right + dir.y * Up;
		public PlaneOrths RebaseOrths(PlaneOrths newBasisSystem, bool printResult = false)
		{
			var orths  = new PlaneOrths(newBasisSystem.InverseVector(Right), newBasisSystem.InverseVector(Up));
			if (printResult) Debug.Log($" [{this}x{newBasisSystem}={orths}] ");
			return orths;
		}
		public PlaneOrths RebaseOrths(BlockFaceDirection initialFace, BlockFaceDirection targetFace)
		{
            FaceOrths localOrths = new FaceOrths(initialFace.ToRotation() * Quaternion);
            return new PlaneOrths(targetFace.InverseVector(localOrths.Right), targetFace.InverseVector(localOrths.Up));
        }
		public PlaneOrths RotateOrths(float angleInDegrees)
		{
			var rotation = Quaternion.AngleAxis(angleInDegrees, Vector3.forward);
			return new PlaneOrths(rotation * Right, rotation * Up);
		}
		public static PlaneOrths operator* (Quaternion rotation , PlaneOrths orths) => new PlaneOrths(rotation * orths.Right, rotation * orths.Up);		
    }
	public readonly struct FaceOrths
	{
		public readonly Vector3 Right;
		public readonly Vector3 Up;
        public override string ToString()
        {
            return $"{Right}x{Up}";
        }
		public FaceOrths(Vector3 right, Vector3 up)
		{
			this.Right = right;
			this.Up = up;
		}
        public FaceOrths(Quaternion rot)
		{
			Right = rot * Vector3.right;
			Up = rot * Vector3.up;
		}
		public FaceOrths(BlockFaceDirection face) : this(face.Rotation) { }
		public Vector2 InverseVector(Vector3 dir) => new (Vector3.Dot(dir, Right), Vector3.Dot(dir,Up));
		public Vector3 TransformVector(Vector2 dir) => Right * dir.x + Up * dir.y;
		public static FaceOrths Default => new FaceOrths(Vector3.right, Vector3.up);
        public static FaceOrths operator *(Quaternion rotation, FaceOrths orths) => new FaceOrths(rotation * orths.Right, rotation * orths.Up);
    }
}
