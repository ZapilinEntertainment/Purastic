using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class ConnectedAndLockedPinsContainer
	{
		public int PairsCount { get; private set; } = 0;
		public readonly ICuttingPlane BasementCutPlane;
		public readonly List<FitElementPlaneAddress> NewBlockConnectedPins = new(), BasementConnectedPins = new();

		public ConnectedAndLockedPinsContainer(ICuttingPlane plane) {
            BasementCutPlane = plane;
		}
		public void AddPair(ConnectingPin landingPin, ConnectingPin newPin)
		{
			NewBlockConnectedPins.Add(newPin.PlaneAddress);
			BasementConnectedPins.Add(landingPin.PlaneAddress);
			PairsCount++;			
		}
		
	}
}
