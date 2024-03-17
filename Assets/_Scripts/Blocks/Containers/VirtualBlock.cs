using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class VirtualBlock // must be class, not struct
    {
        public Vector3 LocalPosition { get; private set; }
        public Quaternion Rotation { get; private set; }
        public readonly BlockProperties Properties;

        public VirtualBlock(Vector3 localPos, PlacingBlockInfo info)
        {
            this.LocalPosition = localPos;
            this.Properties = info.Properties;
            this.Rotation = info.Rotation;
        }
        public VirtualBlock (Vector3 localPos, Quaternion rotation, BlockProperties properties)
        {
            this.LocalPosition = localPos;
            this.Rotation = rotation;
            this.Properties = properties;
        }
        public VirtualBlock(VirtualBlock block)
        {
            this.LocalPosition = block.LocalPosition;
            this.Properties = block.Properties;
            this.Rotation = block.Rotation;
        }
        public void Reposition(Vector3 newPos) => LocalPosition = newPos; 
        public Vector3 FacePositionToModelPosition(Vector2 facePoint, BlockFaceDirection face)
        {
            return TransformNormalizedPoint(face.GetNormalizedZeroPoint()) + Rotation * face.TransformPoint(facePoint);
        }
        public Vector3 TransformPoint(Vector3 inBlockPosition) => LocalPosition + Rotation * inBlockPosition;
        public Vector3 TransformNormalizedPoint(Vector3 pos) => TransformNormalizedPoint(pos.x, pos.y, pos.z);
        public Vector3 TransformNormalizedPoint(float x, float y, float z)
        {
            var size = Properties.ModelSize;
            return LocalPosition + Rotation * new Vector3(0.5f * size.x * x, 0.5f * size.y * y, 0.5f * size.z * z);
        }
        public Vector3 GetFaceZeroPointInLocalSpace(BlockFaceDirection face) => TransformNormalizedPoint(face.GetNormalizedZeroPoint());
        
    }
}
