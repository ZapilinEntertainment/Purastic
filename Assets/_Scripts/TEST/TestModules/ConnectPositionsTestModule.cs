using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class ConnectPositionsTestModule : RectTestModule
    {
        private RectDrawer _projectionDrawer, _mirrorDrawer;
        protected override void OnBlockPositioned(VirtualBlock block)
        {
            base.OnBlockPositioned(block); // lock pins on plate
            var face = ProjectionFace.Mirror();
            _projectionDrawer = RectDrawer.CreateRectDrawer(Baseplate, face, block, Color.Lerp(Color.red, Color.white, 0.5f), 0.2f);
            _mirrorDrawer = RectDrawer.CreateRectDrawer(Baseplate, face, ProjectionRect.ProjectToPlane(ProjectionFace, face), Color.red, 0.15f);
        }
#if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            _projectionDrawer?.DrawCorners();
            _mirrorDrawer?.DrawCorners();
        }
#endif
    }
}
