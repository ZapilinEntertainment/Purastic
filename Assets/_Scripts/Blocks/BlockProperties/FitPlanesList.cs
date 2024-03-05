using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class FitPlanesList
	{
        private FitPlane[] _planes;
        public IReadOnlyCollection<FitPlane> Planes => _planes;
    }
}
