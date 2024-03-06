using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum FitType : byte
	{
	  Knob, Slot
	}
	public static class FitTypeExtension
	{
		public static PinConnectionResult GetConnectResult(this FitType type, FitType otherType)
		{
			if (type == FitType.Knob)
			{
				if (type == FitType.Knob) return PinConnectionResult.Blocked;
				else return PinConnectionResult.Connected;
			}
			else
			{
				if (type == FitType.Slot) return PinConnectionResult.Connected;
				else return PinConnectionResult.NoResult;
			}
		}
	}
}
