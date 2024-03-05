using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public interface IBlocksHost : IColliderOwner
	{
		public int ID { get; }
		public Transform ModelsHost { get; }
        public GameObject CollidersHost { get; }
        public Action<int> OnBlockPlacedEvent { get; set; }
        public IReadOnlyCollection<BlockProperties> GetBlocks();

		public bool TryPinDetail(FitPosition position, BlockProperties block);
		public FitPosition PointToPin(int colliderID, Vector3 point);
    }
}
