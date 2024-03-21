using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public readonly struct PlaneProviderPosition
	{
        public readonly int BlockID;
        public readonly byte BlockSubplaneID;
        public readonly Vector2 CutPlaneZeroPos;
        public readonly BlockFaceDirection Face;
        public readonly PlaneOrths CutPlaneSpaceOrths; // orths in cut plane (plane rotation)

        public Quaternion Rotation => CutPlaneSpaceOrths.Quaternion;

        public PlaneProviderPosition(int blockID, byte subplaneId, BlockFaceDirection face, Vector2 cutPlaneZeroPos, PlaneOrths cutPlaneOrths)
        {
            BlockID = blockID;
            BlockSubplaneID = subplaneId;
            Face = face;
            CutPlaneZeroPos = cutPlaneZeroPos;
            CutPlaneSpaceOrths = cutPlaneOrths;
        }
        public Vector2 PlanePositionToCutPlanePosition(Vector2 planePos) => CutPlaneZeroPos + planePos.x * CutPlaneSpaceOrths.Right + planePos.y * CutPlaneSpaceOrths.Up;
        public Vector2 CutPlanePositionToPlanePosition(Vector2 cutPlanePos)
        {
            Vector2 dir = cutPlanePos - CutPlaneZeroPos;
            return CutPlaneSpaceOrths.InverseVector(dir);
        }
    }
}
