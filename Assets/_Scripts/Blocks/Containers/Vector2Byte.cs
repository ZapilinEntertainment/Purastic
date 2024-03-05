using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct Vector2Byte
	{
		public readonly byte x, y;
		public static Vector2Byte zero => new Vector2Byte(0, 0);
		public static Vector2Byte one => new Vector2Byte(1, 1);
		public Vector2Byte(byte a, byte b)
		{
			x = a; y = b;
		}
		public Vector2Byte(float a, float b)
		{
			x = (byte)a;
			y = (byte)b;
		}
        public static implicit operator Vector2(Vector2Byte pos) => new Vector2(pos.x,pos.y);
    }
}
