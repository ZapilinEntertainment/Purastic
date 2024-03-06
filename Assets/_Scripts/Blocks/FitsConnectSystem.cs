using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class FitsConnectSystem
	{
		public static bool TryConnect(FitsConnectionZone landingZone, FitsConnectionZone blockZone, out List<LockedPin> lockedPins)
		{
            lockedPins = new();
			foreach (var pin in blockZone.Pins)
			{
				var connectResult = landingZone.TryConnect(pin.FitElement, out var landingpin);
				if (connectResult == PinConnectionResult.NoResult) continue;
				else
				{
					if (connectResult == PinConnectionResult.Blocked)
					{
						return false;
					}
					else
					{
                        lockedPins.Add(new LockedPin(blockZone.CutPlaneID, pin.CutPlaneAddress));
                        lockedPins.Add(new LockedPin(landingZone.CutPlaneID, landingpin.CutPlaneAddress));						
					}
				}
			}
			return lockedPins.Count > 0;
		}
	}
}
