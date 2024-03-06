using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum PinConnectionResult : byte { NoResult, Connected, Blocked}
	public class FitsConnectionZone
	{
		public readonly int CutPlaneID;
		public readonly List<ConnectionPin> Pins;

		public FitsConnectionZone(int cutPlaneId, IReadOnlyCollection<ConnectionPin> fits)
		{
			CutPlaneID = cutPlaneId;
			Pins = new( fits);
		}

		public PinConnectionResult TryConnect(FitElement element, out ConnectionPin usedPin)
		{
			foreach (var pin in Pins)
			{
				if (pin.CutPlanePosition == element.CutPlanePosition)
				{
                    usedPin = pin;
                    var result = pin.FitType.GetConnectResult(element.FitType);
					if (result == PinConnectionResult.NoResult) continue;
					else return result;
				}
			}
			usedPin = default;
			return PinConnectionResult.NoResult;
		}
	}
}
