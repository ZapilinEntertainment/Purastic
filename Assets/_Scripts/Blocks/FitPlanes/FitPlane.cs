using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	// contains info about pins plane
	public abstract class FitPlane
	{
		private readonly IFitPlaneDataContainer _dataContainer;
        public BlockFaceDirection Direction { get; }
		public Vector3 PlaneZeroPos { get; }


        public abstract Vector2Byte GetPinIndex(Vector2 cutPlanePos);
		public abstract Vector2 GetInCutPlanePosition(Vector2Byte pinPos);
        public abstract FitElement GetFitElement(Vector2Byte index);
        public abstract IReadOnlyCollection<FitElement> GetAllFitElements();
		public IFitPlanesDataProvider GetDataProvider() => _dataContainer.ToDataProvider(this);
    }

	public static class FitPlanesConnector
	{
		public static bool TryConnect(Vector2Byte initPosA, Vector2Byte initPosB, int fitPlanesA_hash, int fitPlanesB_hash, out FitInfo fitInfo)
		{
			FitPlane fitPlaneA = FitPlanesConfigsDepot.LoadConfig(fitPlanesA_hash),
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

	public interface IFitPlaneDataContainer 
		// contains info only about pins
	{
        public int GetHashCode();
        public Vector2Byte GetFitIndex(Vector2 planedPos);
		public IFitPlanesDataProvider ToDataProvider(Vector2 zeroPoint, PlacedBlockRotation rotation);
    }
}
