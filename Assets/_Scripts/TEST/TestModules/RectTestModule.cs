using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public sealed class RectTestModule : BlockPositionTestModule
	{
		private IReadOnlyCollection<ConnectingPin> _lockedPins = null;

        protected override void OnBlockPositioned(VirtualBlock block)
        {
            if (_lockedPins != null) Baseplate.UnlockPlateZone(_lockedPins);

            var projectionFace = BlockFaceDirection.Down.Inverse();
            var rotation = block.Rotation * projectionFace.ToRotation();
            var faceOrths = new FaceOrths(rotation);
            var rect = new AngledRectangle(
                block.GetFaceZeroPointInLocalSpace(projectionFace),
                block.Properties.GetProjectionSize(projectionFace),
                faceOrths.ToPlaneOrths(projectionFace.Normal)
                );            
            Baseplate.LockPlateZone(rect, out _lockedPins);
            DebugOutputUtility.LogObjects(_lockedPins);
            
           // Baseplate.LockPlateZone(Utilities.ProjectBlock(BlockFaceDirection.Up, block), out _lockedPins);
        }
    }
}
