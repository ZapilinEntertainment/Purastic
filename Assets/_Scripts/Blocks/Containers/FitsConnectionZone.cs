using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum PinConnectionResult : byte { NoResult, Connected, Blocked}
	public class FitsConnectionZone
	{
		public readonly List<ConnectingPin> Pins;

		public FitsConnectionZone(int cutPlaneId, IReadOnlyCollection<ConnectingPin> fits)
		{
			Pins = new( fits);
		}

		public PinConnectionResult TryConnect(FitElement element, out ConnectingPin usedPin)
		{
			foreach (var pin in Pins)
			{
				if (pin.CutPlanePosition == element.Position)
				{
					
                    usedPin = pin;
                    var result = pin.FitType.GetConnectResult(element.FitType);
					if (result == PinConnectionResult.NoResult) continue;
					else return result;
				}
				//else Debug.Log($"{pin.CutPlanePosition} : {element.Position}");
			}
			usedPin = default;
			return PinConnectionResult.NoResult;
		}
	}
}
