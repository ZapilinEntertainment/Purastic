using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class FitsGridConfig : IFitPlaneConfiguration
	{
		public readonly byte Width =1, Length = 1;
		public readonly FitType FitType;
        public Vector2Byte Size => new Vector2Byte(Width, Length);


		public static Vector2 IndexToPosition(Vector2Byte index) => IndexToPosition(index.x, index.y);
		public static Vector2 IndexToPosition(byte x, byte y) => new ((x + 0.5f) * GameConstants.BLOCK_SIZE, (y + 0.5f) * GameConstants.BLOCK_SIZE);

        public FitsGridConfig(FitType fitType, Vector2 modelSize) : this(fitType, (int)(modelSize.x / GameConstants.BLOCK_SIZE), (int)(modelSize.y / GameConstants.BLOCK_SIZE)) { }
        public FitsGridConfig(FitType fitType, int width, int length)
		{
            Width = (byte)Mathf.Clamp(width, 1, 255);
            Length = (byte)Mathf.Clamp(length, 1, 255);
            FitType = fitType;
        }

        public FitsGridConfig(FitType fitType, byte width, byte length)
		{
			Width = width;
			Length = length;
			FitType= fitType;
		}
        public override int GetHashCode() => (Width, Length).GetHashCode();

        public Vector2Byte GetFitIndex( Vector2 planePos)
        {
			float x = Mathf.Clamp( planePos.x / GameConstants.BLOCK_SIZE, 0, Width),
				y = Mathf.Clamp( planePos.y / GameConstants.BLOCK_SIZE,0,Length);
			return new Vector2Byte(x, y);
        }
		public Vector2 GetFitPosition(Vector2Byte index) => IndexToPosition(index);
		public Vector2 ToSize() => new (Width * GameConstants.BLOCK_SIZE, Length * GameConstants.BLOCK_SIZE);

        public FitElement GetFitElement(Vector2Byte index) => new FitElement(
			FitType,
            FitElementSpace.Plane,
			GetFitPosition(index)
			);

		virtual public IFitPlaneDataProvider ToDataProvider(Vector2 zeroPoint, Rotation2D rotation) => new GridDataProvider(this, zeroPoint, rotation);
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect)
        {
            var list = new List<ConnectingPin>();
            for (byte i = 0; i < Width; i++)
            {
                for (byte j = 0; j < Length; j++)
                {
                    Vector2 pos = IndexToPosition(i, j);
                    if (rect.Contains(pos)) list.Add(new ConnectingPin(new FitElement(FitType, FitElementSpace.Plane, pos), new FitElementPlaneAddress(0, new Vector2Byte(i, j))));
                }
            }
            return list;
        }
        public IReadOnlyCollection<FitElement> GetAllPins()
        {
            int width = Width, length = Length;
            var list = new FitElement[width * length];
            for (byte i = 0; i < width; i++)
            {
                for (byte j = 0; j < length; j++)
                {
                    list[i * length + j] = new FitElement(FitType, FitElementSpace.Plane, IndexToPosition(i, j));                    
                }
            }
            return list;
        }

        public Vector2 GetLocalPoint(Vector2Byte index) => IndexToPosition(index);

        public bool TryGetLocalPoint(Vector2Byte index, out Vector2 pos)
        {
            if (index.x < Width && index.y < Length)
            {
                pos = IndexToPosition(index);
                return true;
            }
            else
            {
                pos = default;
                return false;
            }
        }
        public Rect ToRect(Vector2 zeroPos) => new Rect(zeroPos.x, zeroPos.y, Width, Length);
        public FitElementPlanePosition[] GetAllPinsInPlaneSpace()
        {
            var pins = new FitElementPlanePosition[Width * Length];
            for (byte i = 0; i < Width; i++)
            {
                for (byte j = 0; j < Length; j++)
                {
                    pins[i * Length + j] = new FitElementPlanePosition(i, j, IndexToPosition(i, j));
                }
            }
            return pins;
        }
    }
}
