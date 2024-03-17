using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PlacingBlockInfo
	{
		public readonly FitElementPlaneAddress InitialPin;
		public readonly BlockProperties Properties;
		public readonly BlockFaceDirection ConnectFace;
		public readonly Quaternion Rotation;

		public PlacingBlockInfo(FitElementPlaneAddress initialPin,BlockProperties properties, BlockFaceDirection face, Quaternion rotation)
		{
            InitialPin = initialPin;
            Properties = properties;
            ConnectFace = face;
			Rotation = rotation;
		}
        public PlacingBlockInfo(BlockProperties properties)
        {
            InitialPin = new(1, Vector2Byte.zero);
            Properties = properties;
            ConnectFace = GameConstants.DefaultPlacingFace;
			Rotation = Quaternion.identity;
        }
		public PlacingBlockInfo ChangeProperties(BlockProperties properties) => new(InitialPin, properties, ConnectFace, Rotation);

        public Vector3 GetBlockCenterPosition(Vector3 initialPinAddressLocalPosition)
		{
            var plane = Properties.GetPlanesList().GetFitPlane(InitialPin.SubPlaneId);
            var virtualBlock = new VirtualBlock(Vector3.zero, this);

            Vector2 planePos = plane.GetFaceSpacePosition(InitialPin.PinIndex);
            return initialPinAddressLocalPosition - virtualBlock.FacePositionToModelPosition(planePos, GameConstants.DefaultPlacingFace);
		}
    }
}
