using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PinPosition
	{
		public readonly int HostID;
		public readonly FitPlane Plane;
		public readonly Vector2Int PinPoint;
		public readonly Vector3 ModelPosition;

		public PinPosition(Vector3 modelPos)
		{
			ModelPosition = modelPos;
			PinPoint = Vector2Int.zero;
			Plane = new();
			HostID = -1;
		}
		public PinPosition(RaycastHit hit)
		{
			HostID = hit.colliderInstanceID;
			ModelPosition = hit.point;
			PinPoint = Vector2Int.zero;
			Plane = new();
		}
	}
}
