using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	// contains info about pins plane
    // just a config. For receiving instance-related data, create IFitPlanesDataProvider with CreateDataProvider
	public class FitPlaneConfig
	{
        
		public readonly IFitPlaneConfiguration PinsConfiguration;
		public readonly FitType FitType;
        public BlockFaceDirection Face { get; }
		public Vector3 ZeroPos { get; }

        // local rotation of plane not needed - there is no such details with rotated pins
        // for vertical rotation, place plane on a suitable face

        public FitPlaneConfig(IFitPlaneConfiguration pinsConfiguration, FitType fitType, BlockFaceDirection face, Vector3 zeroPos)
        {
            PinsConfiguration = pinsConfiguration;
            FitType = fitType;
            Face = face;
            ZeroPos = zeroPos;
        }

        // plate with knobs
        public FitPlaneConfig(FitType fitType, Vector3Int dimensions, FaceDirection direction)
        {
            PinsConfiguration = new FitsGridConfig(fitType, dimensions.x, dimensions.z);
            FitType = fitType;
            Face = new BlockFaceDirection(direction);
            var normalizedZeroPos = direction.ToPlaneRotation() * GameConstants.NormalizedDefaultPlaneZeroPos;
            ZeroPos = PinsConfiguration.GetZeroPos(GameConstants.GetHeight(dimensions.y) * 0.5f * Mathf.Sign(normalizedZeroPos.y));
        }

        public bool TryGetPlaneSpacePosition(Vector2Byte index, out Vector2 pos)
        {
            if (PinsConfiguration.TryGetPlanePoint(index, out var planePos))
            {
                pos = planePos;
                return true;
            }
            else
            {
                pos = Vector2.zero;
                return false;
            }
        }
        
        public Vector2Byte GetPinIndex(Vector2 cutPlanePos) => PinsConfiguration.GetFitIndex(cutPlanePos);
        public Vector2 GetFaceSpacePosition(Vector2Byte index) =>  PinsConfiguration.GetPlanePoint(index);
        public IFitPlaneDataProvider CreateDataProvider(int blockID, byte subplaneID,Vector2 cutPlaneZeroPoint, PlaneOrths orths) => PinsConfiguration.ToDataProvider(blockID, subplaneID, cutPlaneZeroPoint, orths);
        public IFitPlaneDataProvider CreateDataProvider(int blockID, byte subplaneID, VirtualBlock block, BlockFaceDirection face)
        {  
            var rect = Utilities.ProjectBlock(face, block);
            return CreateDataProvider(blockID, subplaneID, rect.Position, rect.Orths);
        }
        public override int GetHashCode()
        {
            return System.HashCode.Combine(PinsConfiguration.GetHashCode(), FitType.GetHashCode(), Face.GetHashCode(), ZeroPos.GetHashCode());
        }
    }
	public interface IFitPlaneConfiguration 
	{
        // contains detail-space planes data
        // no instance data contained
        public int GetHashCode();
        public bool TryGetPlanePoint(Vector2Byte index, out Vector2 pos);
        public Rect ToRect(Vector2 zeroPos);
        public Vector2 GetPlanePoint(Vector2Byte index);
        public Vector2Byte GetFitIndex(Vector2 planedPos);
        public Vector3 GetZeroPos(float height);
        public IFitPlaneDataProvider ToDataProvider(int blockID,byte subplaneId, Vector2 cutPlaneZeroPoint, PlaneOrths orths);
        public FitElementPlanePosition[] GetAllPinsInPlaneSpace(); // not read-only for further transformations
    }
}
