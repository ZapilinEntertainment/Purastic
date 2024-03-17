using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PlaneOrths
	{
		public readonly Vector2 Right; 
		public readonly Vector2 Up;
		public readonly Quaternion Quaternion => Quaternion.LookRotation(Right * Up, Up);
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

		public Vector2 RebaseVector(Vector2 dir) => new Vector2(Vector2.Dot(dir, Right), Vector2.Dot(dir, Up));
		public PlaneOrths RebaseOrths(PlaneOrths newBasisSystem)
		{
			var orths  = new PlaneOrths(newBasisSystem.RebaseVector(Right), newBasisSystem.RebaseVector(Up));
			Debug.Log($" [{this}x{newBasisSystem}={orths}] ");
			return orths;
		}
		public PlaneOrths RotateOrths(float angleInDegrees)
		{
			var rotation = Quaternion.AngleAxis(angleInDegrees, Vector3.forward);
			return new PlaneOrths(rotation * Right, rotation * Up);
		}
    }
	public readonly struct FaceOrths
	{
		public readonly Vector3 Right;
		public readonly Vector3 Up;
        public override string ToString()
        {
            return $"{Right}x{Up}";
        }
        public FaceOrths(Quaternion rot)
		{
			Right = rot * Vector3.right;
			Up = rot * Vector3.up;
		}
		public Vector2 TransformVector(Vector3 dir) => new (Vector3.Dot(dir, Right), Vector3.Dot(dir,Up));
		public PlaneOrths ToPlaneOrths(Vector3 normal)
		{
			var rotation = Quaternion.FromToRotation(Vector3.forward, normal);
			return new PlaneOrths(TransformVector(rotation *  Vector3.right), TransformVector(rotation * Vector3.up));
		}
	}
}
