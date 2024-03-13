using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class VirtualBlock // must be class, not struct
    {
        public Vector3 LocalPosition { get; private set; }
        public PlacedBlockRotation Rotation { get; private set; }
        public readonly BlockProperties Properties;

        public VirtualBlock(Vector3 localPos, PlacingBlockInfo info)
        {
            this.LocalPosition = localPos;
            this.Properties = info.Properties;
            this.Rotation = info.Rotation;
        }
        public VirtualBlock (Vector3 localPos, PlacedBlockRotation rotation, BlockProperties properties)
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
            Vector3 faceZeroPosInModelSpace = TransformNormalizedPoint(face.GetNormalizedZeroPoint());
            return faceZeroPosInModelSpace + face.ToRotation() * (facePoint.x * Vector3.right + facePoint.y * Vector3.up);
        }
        public Vector3 TransformPoint(Vector3 inBlockPosition) => LocalPosition + Rotation.Quaternion * inBlockPosition;
        public Vector3 TransformNormalizedPoint(Vector3 pos) => TransformNormalizedPoint(pos.x, pos.y, pos.z);
        public Vector3 TransformNormalizedPoint(float x, float y, float z)
        {
            var size = Properties.ModelSize;
            return LocalPosition + Rotation.Quaternion * new Vector3(0.5f * size.x * x, 0.5f * size.y * y, 0.5f * size.z * z);
        }
        public Vector3 GetFaceZeroPointInLocalSpace(BlockFaceDirection face) => TransformNormalizedPoint(face.GetNormalizedZeroPoint());
        public IReadOnlyCollection<ConnectingPin> GetAllConnectionPins(BlockFaceDirection face)
        {
            List<ConnectingPin> list = new();
            var planes = Properties.GetPlanesList().Planes;
            for (byte planeID = 0; planeID < planes.Count; planeID++)
            {
                var plane = planes[planeID];
                if (plane.Face == face)
                {
                    var pins = plane.PinsConfiguration.GetAllPinsInPlaneSpace();
                    foreach (var pin in pins)
                    {
                        Vector2 facePosition = plane.PlanePositionToFacePosition(pin.PlanePosition);
                        list.Add(new ConnectingPin(new FitElement(plane.FitType,FitElementSpace.Face, facePosition), new FitElementPlaneAddress(planeID, pin.Index)));
                    }
                }
            }
            return list;
        }
    }
}
