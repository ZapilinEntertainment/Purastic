using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PlacingBlockInfo
	{
		public readonly Vector2Byte InitialPinIndex;
		public readonly BlockProperties Properties;
		public readonly BlockFaceDirection ConnectFace;

		public PlacingBlockInfo(Vector2Byte initialPin,BlockProperties properties, BlockFaceDirection face)
		{
            InitialPinIndex = initialPin;
            Properties = properties;
            ConnectFace = face;
		}
        public PlacingBlockInfo(BlockProperties properties)
        {
            InitialPinIndex = Vector2Byte.zero;
            Properties = properties;
            ConnectFace = GameConstants.DefaultPlacingFace;
        }

        public Vector3 GetBlockCenterPosition(Vector3 initialPinAddressLocalPosition)
		{
			var planes = Properties.GetPlanesList();
			var pinFacePosition = planes.GetFaceSpacePosition(InitialPinIndex, ConnectFace);
			Vector2 pos = pinFacePosition.PlanePos;

			Vector3 bounds = 0.5f * Properties.ModelSize, zeroPosNormalized = ConnectFace.GetNormalizedZeroPoint();
			Vector3 faceCornerPosition = new Vector3(zeroPosNormalized.x * bounds.x, zeroPosNormalized.y * bounds.y, zeroPosNormalized.z * bounds.z);
			Vector3 pinPositionInModelSpace = ConnectFace.TransformPoint(pos) + faceCornerPosition;
			return initialPinAddressLocalPosition - pinPositionInModelSpace;
		}
    }
}
