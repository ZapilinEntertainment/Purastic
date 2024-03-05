using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IFitPlane
	{
		public Vector2Byte GetFitPosition( Vector2 planedPos);
		public int GetHashCode();
		public FitElement GetFitElement(Vector2Byte index);
		public IReadOnlyCollection<FitElement> GetAllFitElements();
	}

	public static class FitPlanesConnector
	{
		public static bool TryConnect(Vector2Byte initPosA, Vector2Byte initPosB, int fitPlanesA_hash, int fitPlanesB_hash, out FitInfo fitInfo)
		{
			IFitPlane fitPlaneA = FitPlanesConfigsDepot.LoadConfig(fitPlanesA_hash),
				fitPlaneB = FitPlanesConfigsDepot.LoadConfig(fitPlanesB_hash);
			FitElement pointA = fitPlaneA.GetFitElement(initPosA), pointB = fitPlaneB.GetFitElement(initPosB);
			if (pointA.CanConnectWith(pointB))
			{
				fitInfo = new FitInfo(pointA.FitType, pointB.FitType, initPosA, initPosB, Vector2Byte.one);
				return true;
			}
			else
			{
				fitInfo = new();
				return false;
			}
		}
	}
}
