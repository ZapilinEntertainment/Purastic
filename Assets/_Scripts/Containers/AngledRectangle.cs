using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct AngledRectangle
	{
		public readonly Rect Rect;
		public readonly Vector2 Up, Right;

		public AngledRectangle(Rect rect, Vector2 up, Vector2 right)
		{
			Rect = rect;
			Up= up;
			Right= right;
		}

		public Vector2 BottomLeft => Rect.min;
		public Vector2 BottomRight => Rect.min + Right * Rect.width;
		public Vector2 TopLeft => Rect.min + Up * Rect.height;
		public Vector2 TopRight => BottomRight + Up * Rect.width;
		public Vector2 Center => Rect.min + 0.5f * Rect.width * Right + 0.5f * Rect.height * Up;

		public LineSegment[] GetBorders() => new LineSegment[4] {
			new LineSegment(TopLeft, TopRight),
			new LineSegment(TopRight, BottomRight),
			new LineSegment(BottomRight, BottomLeft),
			new LineSegment(BottomLeft, TopLeft)
		};

		public LineSegment GetVerticalCenterSegment()
		{
			Vector2 halfWidth = 0.5f * Rect.width * Right;
			return new LineSegment(BottomLeft + halfWidth, TopLeft + halfWidth);
		}

		public bool Contains(Vector2 point)
		{
			Vector2 dir = point - Rect.min;
			Vector2 localPos = new (Vector2.Dot(dir, Right), Vector2.Dot(dir, Up));
			return Rect.Contains(localPos);
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
