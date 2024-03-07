using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public enum RotationStep : byte
    {
        Degree90
    }
    public readonly struct Rotation2D
	{
		public readonly RotationStep RotationStep;
		public readonly sbyte StepsCount;
        public bool IsDefaultRotation => StepsCount == 0;

        public Rotation2D(RotationStep step, sbyte stepsCount)
        {
            RotationStep = step;
            StepsCount = stepsCount;
        }
        public Rotation2D Inverse()
        {
            //90 deg
            sbyte val;
            if (StepsCount == 0) val = (sbyte)(StepsCount + 2);
            else val = (sbyte)(StepsCount * -1);
            return new Rotation2D(RotationStep, val);
        }
        public static Rotation2D NoRotation => new Rotation2D();
        public static Rotation2D SquareRotation(sbyte stepsCount) => new Rotation2D(RotationStep.Degree90, stepsCount);
	}
    public static class Rotation2DExtension
    {
        public static float ToEulerAngle(this Rotation2D rot)
        {
            return rot.StepsCount * 90f;
        }
        public static (Vector2 up, Vector2 right) CreateOrths(this Rotation2D rot)
        {
            var rotation = Quaternion.AngleAxis(rot.ToEulerAngle(), Vector3.forward);
            return (rotation * Vector2.up, rotation * Vector2.right);
        }
        public static BlockFaceDirection TransformFace(this Rotation2D rotation, BlockFaceDirection face)
        {
            //horizontal rotation
            var direction = face.Direction;
            sbyte stepsCount = rotation.StepsCount;
            if (stepsCount == 0) return face;
            else
            {
                stepsCount %= 3;
                switch (stepsCount)
                {
                    case -2: direction = direction.RotateLeft().RotateLeft();break;
                    case -1: direction = direction.RotateRight().RotateLeft();break;
                    case 1: direction = direction.RotateRight(); break;
                    case 2: direction = direction.RotateRight().RotateRight();break;
                }
            }
            return new BlockFaceDirection(direction);
        }
    }
}
