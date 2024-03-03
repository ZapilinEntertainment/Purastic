using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IBlocksHost : IColliderOwner
	{
		public int ID { get; }
        public GameObject CollidersHost { get; }

		public bool TryPinDetail(PinPosition position, Block block);
		public PinPosition PointToPin(Vector3 point);
    }
}
