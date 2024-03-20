using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public sealed class RectTestModule : BlockPositionTestModule
	{
        [SerializeField] private VisualMaterialType _materialtype = VisualMaterialType.Plastic;
        private Vector3 rectPos;
		private IReadOnlyCollection<ConnectingPin> _lockedPins = null;
        protected override VisualMaterialType MaterialType => _materialtype;

        protected override void OnBlockPositioned(VirtualBlock block)
        {
            if (_lockedPins != null) Baseplate.UnlockPlateZone(_lockedPins);

            var projectionFace = BlockFaceDirection.Down.Inverse();
            var rect = Utilities.ProjectBlock(projectionFace, block,true);
            //Debug.Log(rect);
            Baseplate.LockPlateZone(rect, out _lockedPins);

            Vector3 localPos = Baseplate.GetPlatePlane().CutPlaneToLocalPos(rect.Position);
            rectPos = Baseplate.TransformPosition(localPos);

            DebugOutputUtility.LogObjects(_lockedPins);
            
           // Baseplate.LockPlateZone(Utilities.ProjectBlock(BlockFaceDirection.Up, block), out _lockedPins);
        }
#if UNITY_EDITOR
        override protected void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (Application.isPlaying && enabled)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(rectPos, rectPos + 2f * Vector3.up);
            }
        }
#endif
    }
}
