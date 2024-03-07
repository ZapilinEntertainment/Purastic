using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PlacingBlockInfo
	{
		public readonly BlockProperties Properties;
		public readonly PlacedBlockRotation Rotation;

		public PlacingBlockInfo(BlockProperties properties, PlacedBlockRotation rotation)
		{
			this.Properties = properties;
			this.Rotation = rotation;
		}

		public Vector3 CalculateLocalPosition(Vector3 contactPoint, FitElementPlaneAddress contactingPinAddress, BlockFaceDirection basementContactFace)
		{
			Vector2 pinOffset = Properties.GetPlanesList().GetFitPlane(contactingPinAddress.PlaneContainerID).GetLocalPosition(contactingPinAddress.PinIndex);

        }
	}
}
