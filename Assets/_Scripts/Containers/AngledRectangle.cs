using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct AngledRectangle
	{
		public readonly Rect Rect;
		public readonly Rotation2D Rotation;
        public override string ToString()
        {
            return $"[{Rect}, {Rotation}]";
        }

        public AngledRectangle(Rect rect, Rotation2D rotation)
		{
			Rect = rect;
			Rotation = rotation;
		}

		public AngledRectangle ToPlaneSpace(Vector2 planeZeroPos, Rotation2D hostRotation) => new AngledRectangle(
            new Rect(Rect.position - planeZeroPos, Rect.size),
            hostRotation.FaceToPlane(Rotation)
            );
		public Vector2 BottomLeft => Rect.min;
		public Vector2 BottomRight => Rect.min + Rotation.Right * Rect.width;
		public Vector2 TopLeft => Rect.min + Rotation.Up * Rect.height;
		public Vector2 TopRight => BottomRight + Rotation.Up * Rect.width;
		public Vector2 Center
		{
			get
			{
				var orths = Rotation.CreateOrths();
				return Rect.min + 0.5f * Rect.width * orths.right + 0.5f * Rect.height * orths.up;
			}
		}

		public LineSegment[] GetBorders() => new LineSegment[4] {
			new LineSegment(TopLeft, TopRight),
			new LineSegment(TopRight, BottomRight),
			new LineSegment(BottomRight, BottomLeft),
			new LineSegment(BottomLeft, TopLeft)
		};

		public LineSegment GetVerticalCenterSegment()
		{
			Vector2 halfWidth = 0.5f * Rect.width * Rotation.Right;
			return new LineSegment(BottomLeft + halfWidth, TopLeft + halfWidth);
		}

		public bool Contains(Vector2 point)
		{
			if (Rotation.IsDefaultRotation) return Rect.Contains(point);
			else
			{
				Vector2 dir = point - Rect.position;
				var orths = Rotation.CreateOrths();
				Vector2 localPos = new(Vector2.Dot(dir, orths.right), Vector2.Dot(dir, orths.up));
				return Rect.Contains(localPos + Rect.position);
			}
		}
		public bool IsIntersects(AngledRectangle other)
		{
            LineSegment[] myBorders = GetBorders(), otherBorders = other.GetBorders();
			for (int i =0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (myBorders[i].IsIntersect(otherBorders[j])) return true;
				}
			}

			return Contains(other.BottomLeft) || Contains(other.BottomRight) || Contains(other.TopLeft) || Contains(other.TopRight) ||
				other.Contains(BottomLeft) || other.Contains(BottomRight) || other.Contains(TopLeft) || other.Contains(TopRight);
		}
	}
}
