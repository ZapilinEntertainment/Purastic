using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [System.Serializable]
    public enum RotationStep : byte
    {
        Degree90
    }

    [System.Serializable]
    public readonly struct Rotation2D
    {
        public readonly RotationStep RotationStep;
        public readonly sbyte StepsCount;
        public bool IsDefaultRotation => StepsCount == 0;
        public Vector2 Right => ToQuaternion() * Vector2.right;
        public Vector2 Up => ToQuaternion() * Vector2.up;

        public static Rotation2D NoRotation => new Rotation2D();
        public static Rotation2D SquareRotation(sbyte stepsCount) => new Rotation2D(RotationStep.Degree90, stepsCount);
        public static Vector2 operator *(Rotation2D rotation, Vector2 pos) => rotation.ToQuaternion() * pos;
        public static Rotation2D operator *(Rotation2D A, Rotation2D B)
        {
            return new Rotation2D(RotationStep.Degree90, (sbyte)Mathf.Clamp(A.StepsCount + B.StepsCount, -2, 2));
        }
        public override string ToString() => $"{RotationStep}:{StepsCount}";
        #region equality
        public override bool Equals(object obj) => obj is Rotation2D other && this.Equals(other);
        public bool Equals(Rotation2D p) => RotationStep == p.RotationStep && StepsCount == p.StepsCount;
        public override int GetHashCode() => (RotationStep,StepsCount).GetHashCode();
        public static bool operator ==(Rotation2D lhs, Rotation2D rhs) => lhs.Equals(rhs);
        public static bool operator !=(Rotation2D lhs, Rotation2D rhs) => !(lhs == rhs);
        #endregion


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
        public Rotation2D FaceToPlane(Rotation2D faceRotation)
        {
            if (StepsCount == faceRotation.StepsCount) return faceRotation;
            else
            {
                return new Rotation2D(RotationStep.Degree90, (sbyte)-StepsCount);
            }
        }

        public float ToEulerAngle() => StepsCount* 90f;
        public Vector2 Rotate(Vector2 dir) => ToQuaternion() * dir;
        public Quaternion ToQuaternion() => Quaternion.AngleAxis(ToEulerAngle(), Vector3.back);       
       

        public BlockFaceDirection TransformFace(BlockFaceDirection face)
        {
            //horizontal rotation
            var direction = face.Direction;
            sbyte stepsCount = StepsCount;
            if (stepsCount == 0) return face;
            else
            {
                stepsCount %= 3;
                switch (stepsCount)
                {
                    case -2: direction = direction.RotateLeft().RotateLeft(); break;
                    case -1: direction = direction.RotateRight().RotateLeft(); break;
                    case 1: direction = direction.RotateRight(); break;
                    case 2: direction = direction.RotateRight().RotateRight(); break;
                }
            }
            return new BlockFaceDirection(direction);
        }
        public Vector2 InverseDirection(Vector2 dir)
        {
            var orths = CreateOrths();
            return new Vector2(Vector2.Dot(dir, orths.right), Vector2.Dot(dir, orths.up));
        }

        public (Vector2 up, Vector2 right) CreateOrths()
        {
            var rotation = ToQuaternion();
            return (rotation * Vector2.up, rotation * Vector2.right);
        }
    }
}
