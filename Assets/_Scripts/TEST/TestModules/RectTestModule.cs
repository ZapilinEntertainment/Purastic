using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public class RectTestModule : BlockPositionTestModule
	{
        protected struct RectDrawer
        {
            public readonly Vector3 ZeroPos, OnePos;
            public readonly AngledRectangle Rect;
            private readonly Vector3 _faceNormal;

            public RectDrawer(IBlocksHost host, ICuttingPlane plane, VirtualBlock block)
            {
                _faceNormal = plane.Face.Normal;
                Rect = Utilities.ProjectBlock(plane.Face, block);
                ZeroPos = host.TransformPosition(plane.CutPlaneToLocalPos(Rect.Position));
                OnePos = host.TransformPosition(plane.CutPlaneToLocalPos(Rect.TopRight));
            }
            public RectDrawer(IBlocksHost host, BlockFaceDirection face, VirtualBlock block) : this(host, face, Utilities.ProjectBlock(face, block)) { }
            public RectDrawer(IBlocksHost host, BlockFaceDirection face, AngledRectangle rect)
            {
                _faceNormal = face.Normal;
                Rect = rect;
                ZeroPos = host.TransformPosition(face.TransformVector(Rect.Position));
                var orths = new FaceOrths(face);
                OnePos = host.TransformPosition(face.TransformVector(Rect.TopRight));
            }
            public void DrawGizmos()
            {
                Gizmos.DrawLine(ZeroPos, ZeroPos + 2f * _faceNormal);
                Gizmos.DrawLine(OnePos, OnePos + 1f * _faceNormal);
            }
        }

        [SerializeField] private VisualMaterialType _materialtype = VisualMaterialType.Plastic;
        private RectDrawer _rectDrawerByPlane, _rectDrawerByFace;
		private IReadOnlyCollection<ConnectingPin> _lockedPins = null;

        protected BlockFaceDirection ProjectionFace { get; private set; }
        protected AngledRectangle ProjectionRect => _rectDrawerByPlane.Rect;
        protected override VisualMaterialType MaterialType => _materialtype;

        protected override void OnBlockPositioned(VirtualBlock block)
        {
            if (_lockedPins != null) Baseplate.UnlockPlateZone(_lockedPins);

            var plane = Baseplate.GetPlatePlane();
            ProjectionFace = plane.Face;
            _rectDrawerByPlane = new RectDrawer(Baseplate, plane, block);
            _rectDrawerByFace = new RectDrawer(Baseplate, ProjectionFace, block);
            Debug.Log($"{ProjectionFace}:{_rectDrawerByPlane.Rect}");
            Debug.Log($"{_rectDrawerByPlane.Rect.TopRight}");
            Baseplate.LockPlateZone(_rectDrawerByPlane.Rect, out _lockedPins);
            //DebugOutputUtility.LogObjects(_lockedPins);
           // Baseplate.LockPlateZone(Utilities.ProjectBlock(BlockFaceDirection.Up, block), out _lockedPins);
        }
#if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();
            Gizmos.color = Color.black;
            _rectDrawerByPlane.DrawGizmos();
            Gizmos.color = Color.yellow;
            _rectDrawerByFace.DrawGizmos();
        }
#endif
    }
}
