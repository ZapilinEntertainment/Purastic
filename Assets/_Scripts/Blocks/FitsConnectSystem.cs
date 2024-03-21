using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class FitsConnectSystem
	{
		public static bool TryConnect(ICuttingPlane plane, FitsConnectionZone landingZone, FitsConnectionZone blockZone, out ConnectedAndLockedPinsContainer pinsContainer)
		{
			/*
			var builder = new System.Text.StringBuilder();
            foreach (var pin in landingZone.Pins)
            {
                builder.AppendLine(pin.CutPlanePosition.ToString());
            }
            Debug.Log(builder.ToString());
			builder.Clear();
            foreach (var pin in blockZone.Pins)
            {
                builder.AppendLine(pin.CutPlanePosition.ToString());
            }
            Debug.Log(builder.ToString());
			*/

            pinsContainer = new ConnectedAndLockedPinsContainer(plane);
			foreach (var blockPin in blockZone.Pins)
			{
				var element = blockPin.FitElement;
				var localPos = blockZone.Face.TransformVector(element.Position);
				//var orths = new FaceOrths(blockZone.Face);
				//Debug.Log($"{element.Position} x {orths} = {localPos}");

				var connectResult = landingZone.TryConnect(localPos, element.FitType, out var landingpin);
				//Debug.Log(connectResult);
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
			//Debug.Log(pinsContainer.PairsCount);
            return pinsContainer.PairsCount > 0;
		}
	}
}
