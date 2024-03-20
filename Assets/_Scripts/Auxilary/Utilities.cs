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
            new CuttingPlanePosition(face, DefineCutPlaneCoordinate(block.GetFaceZeroPointInBlockSpace(face), face.Normal));
        public static float DefineCutPlaneCoordinate(Vector3 localPos, Vector3 planeNormal)
        {
            Vector3 projectedPos = Vector3.ProjectOnPlane(localPos, planeNormal);
            return Vector3.Dot(localPos - projectedPos, planeNormal);
        }


        public static AngledRectangle ProjectBlock(BlockFaceDirection receivingFace, VirtualBlock block, bool debugLog = false)
        {
            Vector2 localSize = block.Properties.GetProjectionSize(receivingFace);

            var blockFace = block.LocalToBlockFace(receivingFace);
            var receivingPlaneOrths = new FaceOrths(receivingFace).ToPlaneOrths(receivingFace.Normal);
            var blockFaceOrths = block.GetOrthsOnPlane(blockFace);

            Vector3 blockFaceZeroPoint = block.GetFaceZeroPointInBlockSpace(blockFace.Inverse()); 
            // inverted, because it projects from a mirrored plane
            // example: if we project a block on a UP plane (receivingFace),
            // 1) we take orths from face, that block is faced to plane now:  LocalToBlockFace -> DOWN(blockFace)
            // 2) but for zeroPos we must take corner from the opposite face (even if it doesnt exist in block) - UP(blockFace.Inverse)
            // it is because corner points of opposite planes(like UP and DOWN) are opposite and not match (because of different orths)
            Vector2 zeroPos = receivingFace.LocalToFaceDirection(blockFaceZeroPoint);

            
            if (debugLog)
            {
                Debug.Log($"{receivingFace}:{blockFaceZeroPoint} -> {zeroPos}");
                // Debug.Log(receivingPlaneOrths);
                //Debug.Log(blockFaceOrths);
                //Debug.Log(blockFace);
                // Debug.Log(blockFaceOrths);
            }
            return new(zeroPos, localSize, blockFaceOrths.RebaseOrths(receivingPlaneOrths, debugLog));
        }
        public static AngledRectangle CreatePlaneRect(FitElementPlaneAddress initialPinAddress, Vector2 size, ICuttingPlane plane, float rotationAngle)
        {
            Vector2 initialPinPos = plane.PlaneAddressToCutPlanePos(initialPinAddress);
            const float sz = GameConstants.BLOCK_SIZE;
            Vector2 dir = -0.5f * sz * Vector2.one;
            return new AngledRectangle(
                    initialPinPos + dir,
                    size,
                    PlaneOrths.Default.RotateOrths(rotationAngle)
                    );
        }
    }
}
