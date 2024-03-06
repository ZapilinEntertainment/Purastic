using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum BlockRotationType: byte
	{
		Horizontal90DegStep
	}
	public struct PlacedBlockRotation
	{
		public readonly byte HorizontalRotationStep;
		public readonly BlockRotationType RotationType;

		public Vector3 Forward => Quaternion * Vector3.forward;
		public Quaternion Quaternion => Quaternion.AngleAxis(90f * HorizontalRotationStep, Vector3.up);

		public BlockFaceDirection TransformDirection(BlockFaceDirection face)
		{
            var direction = face.Direction;
            if (direction != FaceDirection.Custom)
			{
				//horizontal rotation
				if (HorizontalRotationStep == 0) return face;
				else
				{
					if (HorizontalRotationStep == 1) direction = direction.RotateRight();
					else
					{
						if (HorizontalRotationStep == 3) direction = direction.RotateLeft();
						else direction = direction.RotateRight().RotateRight();
					}
				}
				return new BlockFaceDirection(direction);
			}
			else return face;
		}
    }
}
