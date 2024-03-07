using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public interface IBlocksHost : IColliderOwner
	{
		public int ID { get; }
        public int RootBlockId { get; }
		public Transform ModelsHost { get; }
        public GameObject CollidersHost { get; }
        public Action<PlacedBlock> OnBlockPlacedEvent { get; set; }
        public IReadOnlyCollection<BlockProperties> GetBlocks();

		public bool TryAddDetail(FitElementStructureAddress position, PlacingBlockInfo placingBlockInfo);
        public bool TryGetFitElementPosition(int colliderID, Vector3 point, out FitElementStructureAddress position);
    }
}
