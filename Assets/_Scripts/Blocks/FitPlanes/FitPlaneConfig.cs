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
		public Vector2 FaceZeroPos { get; }
        // local rotation of plane not needed - there is no such details with rotated pins
        // for vertical rotation, place plane on a suitable face

		public FitPlaneConfig(IFitPlaneConfiguration pinsConfiguration, FitType fitType, BlockFaceDirection face, Vector2 planeZeroPos)
        {
            PinsConfiguration = pinsConfiguration;
            FitType = fitType;
            Face = face;
            FaceZeroPos = planeZeroPos;
        }

        // plate with knobs
        public FitPlaneConfig(FitType fitType, Vector3Int dimensions, FaceDirection direction) : 
            this (new FitsGridConfig(fitType, dimensions.x, dimensions.z), fitType, new BlockFaceDirection(direction), Vector2.zero)
        { }


        public bool TryGetFaceSpacePosition(Vector2Byte index, out Vector2 pos)
        {
            if (PinsConfiguration.TryGetLocalPoint(index, out var planePos))
            {
                pos = PlanePositionToFacePosition(planePos);
                return true;
            }
            else
            {
                pos = Vector2.zero;
                return false;
            }
        }
        public Vector2 PlanePositionToFacePosition(Vector2 planeSpace) => FaceZeroPos + planeSpace;
        
        public Vector2Byte GetPinIndex(Vector2 cutPlanePos) => PinsConfiguration.GetFitIndex(cutPlanePos);
        public Vector2 GetFaceSpacePosition(Vector2Byte index) => PlanePositionToFacePosition( PinsConfiguration.GetLocalPoint(index));
        public IFitPlaneDataProvider CreateDataProvider(Vector2 cutPlaneZeroPoint, Rotation2D rotation) => PinsConfiguration.ToDataProvider(cutPlaneZeroPoint, rotation);
        public IFitPlaneDataProvider CreateDataProvider(VirtualBlock block, BlockFaceDirection face)
        {  
            var rect = Utilities.ProjectBlock(face, block);
            return CreateDataProvider(rect.Rect.position, rect.Rotation);
        }
        public override int GetHashCode()
        {
            return System.HashCode.Combine(PinsConfiguration.GetHashCode(), FitType.GetHashCode(), Face.GetHashCode(), FaceZeroPos.GetHashCode());
        }
    }

	public interface IFitPlaneConfiguration 
	{
        // contains detail-space planes data
        // no instance data contained
        public int GetHashCode();
        public bool TryGetLocalPoint(Vector2Byte index, out Vector2 pos);
        public Rect ToRect(Vector2 zeroPos);
        public Vector2 GetLocalPoint(Vector2Byte index);
        public Vector2Byte GetFitIndex(Vector2 planedPos);
        public IFitPlaneDataProvider ToDataProvider(Vector2 cutPlaneZeroPoint, Rotation2D rotation);
        public FitElementPlanePosition[] GetAllPinsInPlaneSpace(); // not read-only for further transformations
    }
}
