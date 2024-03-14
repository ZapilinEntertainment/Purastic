using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class ConnectedAndLockedPinsContainer
	{
		public int PairsCount { get; private set; } = 0;
		public readonly ICuttingPlane BasementCutPlane;
		public readonly List<ConnectingPin> NewBlockConnectedPins = new(), BasementConnectedPins = new();

		public ConnectedAndLockedPinsContainer(ICuttingPlane plane) {
            BasementCutPlane = plane;
		}
		public void AddPair(ConnectingPin landingPin, ConnectingPin newPin)
		{
			NewBlockConnectedPins.Add(newPin);
			BasementConnectedPins.Add(landingPin);
			//Debug.Log($"{newPin.PlaneAddress} x {landingPin.PlaneAddress}");
			PairsCount++;			
		}

		// when connections are permitted and block was officially placed, it will be assigned a certain id
		public void AssignNewBlockID(int newBlockID)
		{
			int count = NewBlockConnectedPins.Count;
			for (int i = 0; i < count; i++)
			{
				NewBlockConnectedPins[i] = NewBlockConnectedPins[i].ChangeID(newBlockID);
			}
		}
	}
}
