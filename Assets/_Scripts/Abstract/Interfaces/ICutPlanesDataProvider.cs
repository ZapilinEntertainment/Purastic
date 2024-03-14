using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {
	public interface ICutPlanesDataProvider
	{
        public bool TryGetLockZone(CuttingPlanePosition coord, out CuttingPlaneLockZone lockZone);
        public bool TryGetCuttingPlane(CuttingPlanePosition coord, out ICuttingPlane cuttingPlane);
        public ICuttingPlane GetCuttingPlane(int id);
    }
}
