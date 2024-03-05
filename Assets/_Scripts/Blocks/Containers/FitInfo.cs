using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct FitInfo
	{
		public readonly FitType FitTypeA, FitTypeB;
		public readonly Vector2Byte FitIndexA,FitIndexB, ConnectionZoneSize;

		public FitInfo(FitType fitTypeA, FitType fitTypeB, Vector2Byte fitIndexA, Vector2Byte fitIndexB, Vector2Byte connectionZoneSize )
		{
			FitTypeA = fitTypeA;
			FitTypeB = fitTypeB;
			FitIndexA = fitIndexA; 
			FitIndexB = fitIndexB;
			ConnectionZoneSize = connectionZoneSize;
		}
	}
}
