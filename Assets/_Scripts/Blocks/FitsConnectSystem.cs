using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class FitsConnectSystem
	{
		public static bool TryConnect(ICuttingPlane plane, FitsConnectionZone landingZone, FitsConnectionZone blockZone, out ConnectedAndLockedPinsContainer pinsContainer)
		{
			pinsContainer = new ConnectedAndLockedPinsContainer(plane);
			foreach (var blockPin in blockZone.Pins)
			{
				var connectResult = landingZone.TryConnect(blockPin.FitElement, out var landingpin);
                if (connectResult == PinConnectionResult.NoResult) continue;
				else
				{
					if (connectResult == PinConnectionResult.Blocked)
					{
						break;
					}
					else
					{
						pinsContainer.AddPair(landingpin,blockPin);
					}
				}
			}
			return pinsContainer.PairsCount > 0;
		}
	}
}
