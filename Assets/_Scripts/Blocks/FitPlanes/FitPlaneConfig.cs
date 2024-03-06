using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	// contains info about pins plane
	public abstract class FitPlaneConfig
	{
		private readonly IFitPlaneConfiguration _pinsConfiguration;
		public readonly FitType FitType;
        public BlockFaceDirection Face { get; }
		public Vector2 PlaneZeroPos { get; }


        public abstract Vector2Byte GetPinIndex(Vector2 cutPlanePos);
		public abstract Vector2 GetInCutPlanePosition(Vector2Byte pinPos);
		public abstract IReadOnlyCollection<Vector2> GetFitElementsPositions(); 
		public IFitPlanesDataProvider CreateDataProvider(PlacedBlock block) => _pinsConfiguration.ToDataProvider(block.TransformPoint(PlaneZeroPos, Face), block.Rotation);
    }

	/*
	public readonly struct ConnectingFitPlane
	{
		public readonly int FitPlanesContainerHash;
		public readonly BlockFaceDirection Direction;
		public readonly Vector2Byte InitialPinIndex;

		public ConnectingFitPlane(int hash, BlockFaceDirection direction, Vector2Byte index)
		{
			FitPlanesContainerHash= hash;
			Direction= direction;
			InitialPinIndex= index;
		}
	}
	*/

	public interface IFitPlaneConfiguration 
		// contains info only about pins
	{
        public int GetHashCode();
        public Vector2Byte GetFitIndex(Vector2 planedPos);
		public IFitPlanesDataProvider ToDataProvider(Vector2 zeroPoint, PlacedBlockRotation rotation);
    }
}
