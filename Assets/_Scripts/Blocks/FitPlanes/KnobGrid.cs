using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct KnobGrid : IFitPlane
	{
		public readonly byte Width, Length;
		public readonly int HeightInPlates;

        public KnobGrid(int width, int length, int heightInPlates = 1) : this((byte)Mathf.Clamp(width, 1, 255), (byte)Mathf.Clamp(length, 1, 255), heightInPlates)
		{
        }

        public KnobGrid(byte width, byte length, int heightInPlates = 1)
		{
			this.Width = width;
			this.Length = length;
			HeightInPlates = heightInPlates;
			if (HeightInPlates < 1) HeightInPlates = 1;
		}
        public override int GetHashCode() => (Width, Length).GetHashCode();

        public IReadOnlyCollection<FitElement> GetAllFitElements()
        {
			FitElement[] planes = new FitElement[Width * Length];
			const float pinsize = GameConstants.BLOCK_SIZE;
			float halfWidth = Width * 0.5f, halfLength = Length * 0.5f;
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Length; j++)
				{
					planes[i * Length + j] = new FitElement(FitType.Knob, new Vector3((i - halfWidth + 0.5f) * pinsize, GameConstants.GetHeight(HeightInPlates), (j - halfLength + 0.5f) * pinsize));
				}
			}
			return planes;
        }

        public Vector2Byte GetFitPosition( Vector2 projectedPos)
        {
			float x = Mathf.Clamp( projectedPos.x / GameConstants.BLOCK_SIZE, 0, Width),
				y = Mathf.Clamp( projectedPos.y / GameConstants.BLOCK_SIZE,0,Length);
			return new Vector2Byte(x, y);
        }

		public FitElement GetFitElement(Vector2Byte index) => new FitElement(
			FitType.Knob,
			new Vector3(index.x * GameConstants.BLOCK_SIZE, GameConstants.PLATE_THICK, index.y * GameConstants.BLOCK_SIZE)
			);
    }
}
