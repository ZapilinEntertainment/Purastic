using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public class RectTestModule : BlockPositionTestModule
	{

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
            _rectDrawerByPlane = RectDrawer.CreateRectDrawer(Baseplate, plane, block, Color.black, 0.15f);
            _rectDrawerByFace = RectDrawer.CreateRectDrawer(Baseplate, ProjectionFace, block, Color.yellow, 0.1f);
            Debug.Log($"{ProjectionFace}:{_rectDrawerByPlane.Rect}");
            //Debug.Log($"{_rectDrawerByPlane.Rect.TopRight}");
            Baseplate.LockPlateZone(_rectDrawerByPlane.Rect, out _lockedPins);
            //DebugOutputUtility.LogObjects(_lockedPins);
           // Baseplate.LockPlateZone(Utilities.ProjectBlock(BlockFaceDirection.Up, block), out _lockedPins);
        }
#if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();
            _rectDrawerByPlane?.DrawCorners();
            _rectDrawerByFace?.DrawCorners();
        }
#endif
    }
}
