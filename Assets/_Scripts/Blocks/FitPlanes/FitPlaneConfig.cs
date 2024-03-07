using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	// contains info about pins plane
    // just a config. For receiving actual data, create IFitPlanesDataProvider with CreateDataProvider
	public class FitPlaneConfig
	{
		public readonly IFitPlaneConfiguration PinsConfiguration;
		public readonly FitType FitType;
        public BlockFaceDirection Face { get; }
		public Vector2 PlaneZeroPos { get; }

		public FitPlaneConfig(IFitPlaneConfiguration pinsConfiguration, FitType fitType, BlockFaceDirection face, Vector2 planeZeroPos)
        {
            PinsConfiguration = pinsConfiguration;
            FitType = fitType;
            Face = face;
            PlaneZeroPos = planeZeroPos;
        }

        // plate with knobs
        public FitPlaneConfig(FitType fitType, Vector3Int dimensions, FaceDirection direction) : 
            this (new FitsGridConfig(fitType, dimensions.x, dimensions.z), fitType, new BlockFaceDirection(direction), Vector2.zero)
        { }

        public Vector2Byte GetPinIndex(Vector2 cutPlanePos) => PinsConfiguration.GetFitIndex(cutPlanePos);
        public Vector2 GetLocalPosition(Vector2Byte index) => PinsConfiguration.GetLocalPoint(index);
		public IFitPlanesDataProvider CreateDataProvider(PlacedBlock block) => PinsConfiguration.ToDataProvider(
			block.TransformPoint(PlaneZeroPos, Face), 
			block.Rotation.TransformPlaneRotation(Face)
			);
        public IFitPlanesDataProvider CreateDataProvider(Vector2 zeroPoint, Rotation2D rotation) => PinsConfiguration.ToDataProvider(zeroPoint, rotation);

        public override int GetHashCode()
        {
            return System.HashCode.Combine(PinsConfiguration.GetHashCode(), FitType.GetHashCode(), Face.GetHashCode(), PlaneZeroPos.GetHashCode());
        }
    }

	public interface IFitPlaneConfiguration 
		// contains info only about pins
	{
        public int GetHashCode();
        public Vector2 GetLocalPoint(Vector2Byte index);
        public Vector2Byte GetFitIndex(Vector2 planedPos);
		public IFitPlanesDataProvider ToDataProvider(Vector2 zeroPoint, Rotation2D rotation);
        public IReadOnlyCollection<FitElement> GetAllPins();
    }
}
