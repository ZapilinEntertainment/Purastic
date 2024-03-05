using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct FitPosition
	{
		public readonly int BlockId;
		public readonly FitPlaneDirection Direction;
		public readonly Vector2Byte PinIndex;
		public readonly Vector3 ModelPosition;

		public FitPosition(Vector3 modelPos)
		{
			ModelPosition = modelPos;
			PinIndex = Vector2Byte.zero;
			BlockId = -1;
			Direction = FitPlaneDirection.Up;
		}
		public FitPosition(int blockID, FitPlaneDirection direction, Vector2Byte pinIndex, Vector3 modelPos)
		{
			BlockId = blockID;
			Direction = direction;
			PinIndex = pinIndex;
			ModelPosition = modelPos;
		}
	}
}
