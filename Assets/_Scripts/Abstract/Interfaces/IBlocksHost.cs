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

        public bool TryGetBlock(int blockID, out PlacedBlock block);
        public bool CheckZoneForObstruction(int cutPlaneID, AngledRectangle rect);
        public bool TryAddDetail(FitElementStructureAddress position, PlacingBlockInfo placingBlockInfo);
        public bool TryGetFitElementPosition(RaycastHit hit, out FoundedFitElementPosition position);
       

        public Vector3 InverseTransformPosition(Vector3 position);
        public Vector3 TransformPosition(Vector3 position);
        public Vector3 TransformPosition(Vector2 facePos, BlockFaceDirection face) => TransformPosition(face.TransformVector(facePos));
        public Vector3 TransformPosition(Vector2 facePos, ICuttingPlane cutPlane) => TransformPosition(cutPlane.CutPlaneToLocalPos(facePos));

        public VirtualBlock CreateVirtualBlock(ICuttingPlane plane, FitElementFaceAddress faceAddress, PlacingBlockInfo placingInfo);
        public VirtualBlock CreateVirtualBlock(FitElementStructureAddress address, PlacingBlockInfo placingInfo) => CreateVirtualBlock(CutPlanesDataProvider.GetCuttingPlane(address.CutPlaneID), address.ToFaceAddress(), placingInfo);
    }
}
