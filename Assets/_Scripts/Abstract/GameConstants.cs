using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic
{
    public static class GameConstants
    {
        public const float BLOCK_SIZE = PLATE_THICK * PLATES_IN_BLOCK, KNOB_SCALE = 1f, PLATE_THICK = 0.3f;
        public const int PLATES_IN_BLOCK = 3;

        public const float MAX_POINT_CAST_DISTANCE = 100f;
        public static Vector3 NormalizedDefaultPlaneZeroPos => new Vector3(-1f,-1f, 1f );

        public static BlockFaceDirection DefaultPlacingFace=> new BlockFaceDirection(DefaultPlacingDirection);
        private const FaceDirection DefaultPlacingDirection = FaceDirection.Down;

        public static float GetHeight(int platesCount) => (platesCount / PLATES_IN_BLOCK) * BLOCK_SIZE + (platesCount % PLATES_IN_BLOCK) * PLATE_THICK;
    }
}
