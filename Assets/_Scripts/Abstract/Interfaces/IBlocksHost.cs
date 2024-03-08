using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public interface IBlocksHost : IColliderOwner, ILateInitializable
	{
		public int ID { get; }
        public int RootBlockId { get; }
        public Vector3 ZeroPoint { get; }
		public Transform ModelsHost { get; }
        public GameObject CollidersHost { get; }
        public Action<PlacedBlock> OnBlockPlacedEvent { get; set; }
        public IReadOnlyCollection<BlockProperties> GetBlocks();

		public bool TryAddDetail(FitElementStructureAddress position, PlacingBlockInfo placingBlockInfo);
        public bool TryGetFitElementPosition(int colliderID, Vector3 point, out FoundedFitElementPosition position);

        public Vector3 InverseTransformPosition(Vector3 position) => ModelsHost.InverseTransformPoint(position);
        public Vector3 TransformPosition(Vector3 position) => ModelsHost.TransformPoint(position);
    }
}
