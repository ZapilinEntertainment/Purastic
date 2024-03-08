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
    }
}
