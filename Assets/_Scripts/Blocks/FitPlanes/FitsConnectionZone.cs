using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum PinConnectionResult : byte { NoResult, Connected, Blocked}
	public class FitsConnectionZone
	{
		public readonly BlockFaceDirection Face;
		public readonly List<ConnectingPin> Pins;

		public FitsConnectionZone(BlockFaceDirection face, IReadOnlyCollection<ConnectingPin> fits)
		{
			Face = face;
			Pins = new( fits);
		}

		public PinConnectionResult TryConnect(Vector3 pos, FitType fitType, out ConnectingPin usedPin)
		{
			Vector2 facePos = Face.InverseVector(pos);
			foreach (var pin in Pins)
			{
                //Debug.Log($"{pos}x{Face.TransformVector(pin.CutPlanePosition)}");
                if (pin.CutPlanePosition == facePos)
				{					
                    usedPin = pin;
					//Debug.Log($"{fitType}x{pin.FitType}");
                    var result = pin.FitType.GetConnectResult(fitType);
					if (result == PinConnectionResult.NoResult) continue;
					else return result;
				}
			}
			usedPin = default;
			return PinConnectionResult.NoResult;
		}
	}
}
