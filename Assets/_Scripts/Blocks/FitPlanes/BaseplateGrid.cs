using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct BaseplateGrid : IFitPlanesContainer
	{
		public readonly byte Width, Length;

		public BaseplateGrid(byte width, byte length)
		{
			this.Width = width;
			this.Length = length;
		}
        public override int GetHashCode() => (Width, Length).GetHashCode();

        public IReadOnlyCollection<FitPlane> GetFitPlanes()
        {
			FitPlane[] planes = new FitPlane[Width * Length];
			const float pinsize = GameConstants.BLOCK_SIZE;
			float halfWidth = Width * 0.5f, halfLength = Length * 0.5f;
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Length; j++)
				{
					planes[i * Width + j] = new FitPlane(FitType.Knob, new Vector3((i - halfWidth + 0.5f) * pinsize, GameConstants.PLATE_THICK, (j - halfLength + 0.5f) * pinsize));
				}
			}
			return planes;
        }

        
    }
}
