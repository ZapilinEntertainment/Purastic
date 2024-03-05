using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct FitElementPosition
	{
		public readonly int BlockId;
		public readonly BlockFaceDirection Direction;
		public readonly Vector2Byte PinIndex;
		public readonly Vector3 WorldPosition;

		public FitElementPosition(Vector3 modelPos)
		{
			WorldPosition = modelPos;
			PinIndex = Vector2Byte.zero;
			BlockId = -1;
			Direction = default;
		}
		public FitElementPosition(int blockID, BlockFaceDirection direction, Vector2Byte pinIndex, Vector3 position)
		{
			BlockId = blockID;
			Direction = direction;
			PinIndex = pinIndex;
			WorldPosition = position;
		}
	}
}
