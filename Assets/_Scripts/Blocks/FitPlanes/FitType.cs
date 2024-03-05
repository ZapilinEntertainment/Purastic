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
		public static bool CanConnect(this FitType type, FitType otherType)
		{
			return type != otherType;
		}
	}
}
