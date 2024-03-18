using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct AngledRectangle
	{
		public readonly Vector2 Position;
		public readonly Vector2 Size;
		public readonly PlaneOrths Orths;
		public float Width => Size.x;
		public float Height => Size.y;
		public Vector2 Right => Orths.Right;
		public Vector2 Up => Orths.Up;
        public override string ToString()
        {
            return $"[{Position}, {Size.x}x{Size.y}, {Right}x{Up} ]";
        }

		public AngledRectangle(Vector2 zeroPos, Vector2 size, PlaneOrths orths)
		{
			Position = zeroPos; 
			Size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y)) ; // must always be positive - all planes inversion and rotation displays in orths
			Orths = orths;
			//Debug.Log(Orths);
		}
        public AngledRectangle(float posX, float posY, float width, float height, PlaneOrths orths) : this(new Vector2(posX, posY), new Vector2(width, height), orths) { }
        public AngledRectangle ToPlaneSpace(Vector2 planeZeroPos, PlaneOrths planeOrths )
		{
			return new AngledRectangle(Position - planeZeroPos, Size, Orths.RebaseOrths(planeOrths));
		}
		public Vector2 BottomLeft => Position;
		public Vector2 BottomRight => Position + Right * Width;
		public Vector2 TopLeft => Position + Up * Height;
		public Vector2 TopRight => BottomRight + Up * Height;
		public Vector2 Center => Position + 0.5f * Width * Right  + 0.5f *Height * Up;

		public LineSegment[] GetBorders() => new LineSegment[4] {
			new LineSegment(TopLeft, TopRight),
			new LineSegment(TopRight, BottomRight),
			new LineSegment(BottomRight, BottomLeft),
			new LineSegment(BottomLeft, TopLeft)
		};

		public LineSegment GetVerticalCenterSegment()
		{
			Vector2 halfWidth = 0.5f * Width * Right;
			return new LineSegment(BottomLeft + halfWidth, TopLeft + halfWidth);
		}

		public bool Contains(Vector2 point) {
			Vector2 dir = point - Position;
			float x = Vector2.Dot(dir, Right), y = Vector2.Dot(dir, Up);
			bool result =  x >= 0 && x <= Width && y >= 0 && y <= Height;
			if (result)
			{
				//Debug.Log($"{dir}x{Up}={System.Math.Round(Vector2.Dot(dir,Up),2)}");
				//Debug.Log($"{x}:{System.Math.Round(y,2)}");
				return true;
			}
			else return false;

           return new Rect(Position, Size).Contains(point);
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
