using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class RectDrawer
    {
        #region transformers
        private interface IPointTransformer
        {
            public Quaternion Rotation { get; }
            public Vector3 TransformPosition(Vector2 facePos);
        }
        private abstract class AbstractPointTransformer : IPointTransformer
        {
            private readonly IBlocksHost _host;
            private readonly Quaternion _rotation;
            protected IBlocksHost Host => _host;
            public Quaternion Rotation => _rotation;            
            public AbstractPointTransformer(IBlocksHost host, BlockFaceDirection face, PlaneOrths orths)
            {
                _host = host;

                float angle = orths.Quaternion.eulerAngles.z;
                _rotation = _host.ModelsHost.rotation * Quaternion.FromToRotation(Vector3.up, face.Rotation * Vector3.forward) * Quaternion.AngleAxis(angle, face.Normal);
            }
            public abstract Vector3 TransformPosition(Vector2 facePos);
        }
        private sealed class FacePointTransformer : AbstractPointTransformer
        {            
            private readonly BlockFaceDirection Face;
            public FacePointTransformer(IBlocksHost host, BlockFaceDirection face, PlaneOrths orths) :base(host, face, orths) { 
                Face = face;
            }

            override public Vector3 TransformPosition(Vector2 facePos) => Host.TransformPosition(facePos, Face);
        }
        private sealed class CutPlanePointTransformer : AbstractPointTransformer
        {
            private readonly ICuttingPlane Plane;
            public CutPlanePointTransformer(IBlocksHost host, ICuttingPlane plane, PlaneOrths orths) : base(host, plane.Face, orths)
            {
                Plane = plane;
            }

            override public Vector3 TransformPosition(Vector2 facePos) => Host.TransformPosition(facePos, Plane);
        }
        #endregion

        private readonly Color DrawColor= Color.white;
        public readonly Vector3 WorldPos;
        public readonly Vector3 Size;
        public readonly Matrix4x4 Matrix;

        public readonly Vector3 ZeroPos;
        public readonly Vector3 OnePos;
        public readonly Vector3 FaceNormal;
        public readonly AngledRectangle Rect;

        public static RectDrawer CreateRectDrawer(IBlocksHost host, int cutPlaneId, VirtualBlock block, Color color, float height = 1f)
        {
            var plane = host.CutPlanesDataProvider.GetCuttingPlane(cutPlaneId);
            var rect = Utilities.ProjectBlock(plane.Face, block);
            return new RectDrawer(rect, plane.Face, new CutPlanePointTransformer(host, plane, rect.Orths), color, height);
        }
        public static RectDrawer CreateRectDrawer(IBlocksHost host, int cutPlaneId, AngledRectangle rect, Color color, float height = 1f)
        {
            var plane = host.CutPlanesDataProvider.GetCuttingPlane(cutPlaneId);
            return new RectDrawer(rect, plane.Face, new CutPlanePointTransformer(host, plane, rect.Orths),color, height);
        }
        public static RectDrawer CreateRectDrawer(IBlocksHost host, BlockFaceDirection face, AngledRectangle rect, Color color, float height = 1f)
        {            
            return new RectDrawer(rect, face, new FacePointTransformer(host, face, rect.Orths), color, height);
        }
        public static RectDrawer CreateRectDrawer(IBlocksHost host, ICuttingPlane plane, VirtualBlock block, Color color, float height = 1f)
        {
            var rect = Utilities.ProjectBlock(plane.Face, block);
            return new RectDrawer(rect, plane.Face, new CutPlanePointTransformer(host, plane, rect.Orths), color, height);
        }
        public static RectDrawer CreateRectDrawer(IBlocksHost host, BlockFaceDirection face, VirtualBlock block, Color color, float height)
        {
            var rect = Utilities.ProjectBlock(face, block);
            return new RectDrawer(rect, face, new FacePointTransformer(host, face, rect.Orths), color, height);
        }

        private RectDrawer(AngledRectangle rect, BlockFaceDirection face, IPointTransformer transformer, Color color, float height)
        {
            Rect = rect;
            FaceNormal = face.Normal;
            WorldPos = transformer.TransformPosition(rect.Center);
            Size = new Vector3(rect.Width, height, rect.Height);
            Matrix = Matrix4x4.TRS(Vector3.zero, transformer.Rotation, Vector3.one);
            WorldPos = Matrix.inverse.MultiplyVector(WorldPos);

            DrawColor = color;

            ZeroPos = transformer.TransformPosition(rect.Position);
            OnePos = transformer.TransformPosition(rect.TopRight);
        }

        public void DrawRect()
        {
#if UNITY_EDITOR
            Gizmos.color = DrawColor;
            Gizmos.matrix = Matrix;
            Gizmos.DrawWireCube(WorldPos, Size);
#endif
        }
        public void DrawCorners()
        {
#if UNITY_EDITOR
            Gizmos.DrawLine(ZeroPos, ZeroPos + 2f * FaceNormal);
            Gizmos.DrawLine(OnePos, OnePos + 1f * FaceNormal);
#endif
        }
    }
}
