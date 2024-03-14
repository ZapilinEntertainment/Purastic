using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	
	public readonly struct PlacedBlockRotation
	{
		public readonly Rotation2D HorizontalRotation;
		public readonly Rotation2D VerticalRotation;
		public static PlacedBlockRotation NoRotation => new PlacedBlockRotation(Rotation2D.NoRotation, Rotation2D.NoRotation);

		public Vector3 Forward => Quaternion * Vector3.forward;
		public Vector3 Right  => Quaternion * Vector3.right;
		public Quaternion Quaternion
		{
			get
			{
				if (HorizontalRotation.IsDefaultRotation)
				{
					return GetVerticalQuaternion(Vector3.right);
				}
				else
				{
					if (VerticalRotation.IsDefaultRotation) return HorizontalQuaternion;
					else
					{
						return HorizontalQuaternion * GetVerticalQuaternion(HorizontalQuaternion * Vector3.right);
					}
				}
			}
		}
		private Quaternion HorizontalQuaternion => Quaternion.AngleAxis(HorizontalRotation.ToEulerAngle(), Vector3.up);
        private Quaternion GetVerticalQuaternion(Vector3 right) => Quaternion.AngleAxis(HorizontalRotation.ToEulerAngle(), right);

        public override string ToString() => $"h:{HorizontalRotation}, v:{VerticalRotation}";

        public PlacedBlockRotation(Rotation2D horizontalRotation, Rotation2D verticalRotation)
		{
			this.HorizontalRotation = horizontalRotation;
			this.VerticalRotation = verticalRotation;
		}

        public BlockFaceDirection TransformDirection(BlockFaceDirection face)
		{
            var direction = face.Direction;
            if (direction != FaceDirection.Custom && direction != FaceDirection.Undefined)
			{
				return HorizontalRotation.TransformFace(face);
			}
			else return face;
		}
		public BlockFaceDirection InverseDirection(BlockFaceDirection face)
		{
            var direction = face.Direction;
            if (direction != FaceDirection.Custom && direction != FaceDirection.Undefined)
            {
                return HorizontalRotation.Inverse().TransformFace(face);
            }
            else return face;
        }
        public Rotation2D DefinePlaneRotation(BlockFaceDirection face)
		{
			var directionInModel = TransformDirection(face);
			return directionInModel.GetHorizontalRotation();
		}

		public PlacedBlockRotation RotateRight() => new (HorizontalRotation.RotateRight(), VerticalRotation);
        public PlacedBlockRotation RotateLeft() => new(HorizontalRotation.RotateLeft(), VerticalRotation);
		public (Vector3 right,Vector3 up) CreateFaceOrths()
		{
			var quaternion = Quaternion;
			return (quaternion * Vector3.right, quaternion * Vector3.up);
		}
    }
}
