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
            return Vector3.Dot(localPos - projectedPos, planeNormal);
        }
        public static AngledRectangle ProjectBlock(BlockFaceDirection receivingFace, VirtualBlock block, bool debugLog = false)
        {
            Vector2 localSize = block.Properties.GetProjectionSize(receivingFace);
            var blockFace = block.LocalToBlockFace(receivingFace);
            var blockFaceOrths = block.GetOrthsOnPlane(block.LocalToBlockFace(receivingFace)); 
            // because two faces differences only in normal vector,
            // we need to project orths on the landing plane to make correct rect rotation
            Vector2 zeroPos = receivingFace.LocalToFaceDirection(block.LocalPosition) - 0.5f * localSize.x * blockFaceOrths.Right - 0.5f * localSize.y * blockFaceOrths.Up;

            if (debugLog)
            {
                Debug.Log(blockFace);
                Debug.Log(blockFaceOrths);
                Debug.Log($"{block.LocalPosition} -> {receivingFace.LocalToFaceDirection(block.LocalPosition)}");
            }
            return new(zeroPos, localSize, blockFaceOrths);
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
