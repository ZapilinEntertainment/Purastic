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
        public ICutPlanesDataProvider CutPlanesDataProvider { get; }
		public Transform ModelsHost { get; }
        public GameObject CollidersHost { get; }
        public Action<PlacedBlock> OnBlockPlacedEvent { get; set; }
        public IReadOnlyCollection<BlockProperties> GetBlocks();

        public bool CheckZoneForObstruction(int cutPlaneID, AngledRectangle rect);
        public bool TryAddDetail(FitElementStructureAddress position, PlacingBlockInfo placingBlockInfo);
        public bool TryGetFitElementPosition(RaycastHit hit, out FoundedFitElementPosition position);

        public Vector3 InverseTransformPosition(Vector3 position);
        public Vector3 TransformPosition(Vector3 position);
    }
}
