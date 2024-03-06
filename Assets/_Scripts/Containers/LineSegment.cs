using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct LineSegment
	{
		// ax + by + c = 0;
		public readonly float A,B,C;
		public readonly Vector2 StartPoint, EndPoint;

		public LineSegment(Vector2 startPoint, Vector2 endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
			A = EndPoint.y - StartPoint.y;
			B = StartPoint.x - EndPoint.x;
			C = A * StartPoint.x + B * StartPoint.y;
        }

		public bool ContainsX(float x) => CheckContainment(x, StartPoint.x, EndPoint.x);
        public bool ContainsY(float y) => CheckContainment(y, StartPoint.y, EndPoint.y);
        private bool CheckContainment(float val, float a, float b)
		{
            float min = a, max;
            if (b < min)
            {
                max = min;
                min = b;
            }
            else max = b;
			return (val >= min && val <= max);
        }
       
        public bool IsIntersect(LineSegment other)
		{
			float det = A * other.B - other.A * B;
			if (det == 0f) return false; // parallel
			else
			{
				float x = (other.B * C - B * other.C) / det;
				float y = (A * other.C - other.A * C) / det;
				return ContainsX(x) && ContainsY(y) && other.ContainsX(x) && other.ContainsY(y);
            }
		}
	}
}
