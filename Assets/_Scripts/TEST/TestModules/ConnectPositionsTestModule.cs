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
            var face = ProjectionFace.Inverse();
            _projectionDrawer = new RectDrawer(Baseplate, face, block);
            _mirrorDrawer = new RectDrawer(Baseplate, face, ProjectionRect.ProjectToPlane(ProjectionFace, face));
        }
#if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();
            Gizmos.color = Color.Lerp( Color.red, Color.white, 0.5f);
            _projectionDrawer.DrawGizmos();
            Gizmos.color = Color.red;
            _mirrorDrawer.DrawGizmos();
        }
#endif
    }
}
