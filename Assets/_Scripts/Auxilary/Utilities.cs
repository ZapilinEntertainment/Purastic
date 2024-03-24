using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class Utilities
	{

        private static System.Random _random = new();
        public static int GenerateInteger() => _random.Next();
        public static float TrimFloat(float y) => (float)System.Math.Round(y, 5);
        public static Vector2 TrimVector(Vector2 pos) => new Vector2(TrimFloat(pos.x), TrimFloat(pos.y));


        public static CuttingPlanePosition DefineCutPlaneCoordinate(VirtualBlock block, BlockFaceDirection face) => 
            new CuttingPlanePosition(face, DefineCutPlaneCoordinate(block.GetFaceZeroPointInLocalSpace(face), face.Normal));
        public static float DefineCutPlaneCoordinate(Vector3 localPos, Vector3 planeNormal)
        {
            Vector3 projectedPos = Vector3.ProjectOnPlane(localPos, planeNormal);
            return TrimFloat(Vector3.Dot(localPos - projectedPos, planeNormal));
        }


        public static AngledRectangle ProjectBlock(BlockFaceDirection receivingFace, VirtualBlock block, bool debugLog = false)
        {
            Vector2 localSize = block.Properties.GetProjectionSize(receivingFace);

            var blockFace = block.GetContactFace(receivingFace); // ex: non-rotated block connects with UP plane with DOWN plane
            var calculatingFace = blockFace.Inverse();
            // inverted, because it projects from a mirrored plane
            // example: if we project a block on a UP plane (receivingFace),
            // 1) we take orths from face, that block is faced to plane now:  LocalToBlockFace -> DOWN(blockFace)
            // 2) but for zeroPos we must take corner from the opposite face (even if it doesnt exist in block) - UP(blockFace.Inverse)
            // it is because corner points of opposite planes(like UP and DOWN) are opposite and not match (because of different orths)
            PlaneOrths blockFaceOrths = new PlaneOrths(calculatingFace, block.Rotation);
            Vector3 blockFaceZeroPoint = block.GetFaceZeroPointInLocalSpace(calculatingFace);

            // var rect = new AngledRectangle(blockFaceZeroPoint, localSize, blockFaceOrths);
            //return rect.ProjectToPlane(calculatingFace, receivingFace);
            Vector2 zeroPos = receivingFace.InverseVector(blockFaceZeroPoint);
            if (debugLog)
            {
                Debug.Log($"{receivingFace}:{blockFaceZeroPoint} -> {zeroPos}");
                //Debug.Log(rectOrths);
                // Debug.Log(receivingPlaneOrths);
                //Debug.Log($"relativeOrths:{blockFaceOrths}");
                //Debug.Log(blockFace);
                // Debug.Log(blockFaceOrths);
            }
            return new(zeroPos, localSize, blockFaceOrths);
        }

        /* wrong:
        public static AngledRectangle CreatePlaneRect(FoundedFitElementPosition position, IBlocksHost host, BlockProperties properties, Quaternion rotation)
        {
            var cutPlane = host.CutPlanesDataProvider.GetCuttingPlane(position.StructureAddress.CutPlaneID);
            var face = position.StructureAddress.ContactFace;
            return CreatePlaneRect(position.StructureAddress.PlaneAddress, properties.GetProjectionSize(face), cutPlane, new PlaneOrths(face, rotation));
        }
        */
        public static AngledRectangle CreatePlaneRect(FitElementFaceAddress initialPinAddress, Vector2 size, ICuttingPlane plane, float rotationAngle) => CreatePlaneRect(initialPinAddress, size, plane, PlaneOrths.Default.RotateOrths(rotationAngle));
        public static AngledRectangle CreatePlaneRect(FitElementFaceAddress initialPinAddress, Vector2 size, ICuttingPlane plane, PlaneOrths orths)
        {
            Vector2 initialPinPos = plane.FaceAddressToCutPlanePos(initialPinAddress);

            const float sz = GameConstants.BLOCK_SIZE;
            Vector2 dir = -0.5f * sz * Vector2.one;
            return new AngledRectangle(
                    initialPinPos + dir,
                    size,
                    orths
                    );
        }
    }
}
