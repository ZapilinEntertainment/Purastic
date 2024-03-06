using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct FitsGridConfig : IFitPlaneConfiguration
	{
		public readonly byte Width, Length;
		public readonly FitType FitType;
		public static Vector2 IndexToPosition(Vector2Byte index) => new Vector3(index.x * GameConstants.BLOCK_SIZE, GameConstants.PLATE_THICK, index.y * GameConstants.BLOCK_SIZE);

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

        public Vector2Byte GetFitIndex( Vector2 projectedPos)
        {
			float x = Mathf.Clamp( projectedPos.x / GameConstants.BLOCK_SIZE, 0, Width),
				y = Mathf.Clamp( projectedPos.y / GameConstants.BLOCK_SIZE,0,Length);
			return new Vector2Byte(x, y);
        }
		public Vector2 GetFitPosition(Vector2Byte index) => IndexToPosition(index);

        public FitElement GetFitElement(Vector2Byte index) => new FitElement(
			FitType,
			GetFitPosition(index)
			);

		public IFitPlanesDataProvider ToDataProvider(Vector2 zeroPoint, PlacedBlockRotation rotation) => new GridDataProvider(this, zeroPoint, rotation);
    }
}
