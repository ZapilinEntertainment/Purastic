using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class ViewPointInfo
	{
		public readonly Transform ViewPoint;
        public readonly PointViewSettings PointViewSettings;

        public ViewPointInfo(Transform viewPoint, PointViewSettings viewSettings)
		{
			ViewPoint = viewPoint;
            PointViewSettings = viewSettings;
		}
	}
    [System.Serializable]
    public class PointViewSettings
    {
        [field: SerializeField] public float HeightViewCf { get; private set; } = 1f;
        [field: SerializeField] public float HeightSpeedOffsetCf { get; private set; } = 1f;
        [field: SerializeField] public float ZSpeedOffset { get; private set; } = 10f;
    }
}
