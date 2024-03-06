using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    // contains all connect planes info
    // 
	public class FitPlanesConfigList
	{
        private readonly FitPlaneConfig[] _planes;
        public IReadOnlyCollection<FitPlaneConfig> Planes => _planes;

        public FitPlanesConfigList(FitPlaneConfig[] planes)
        {
            _planes = planes;
        }
        public FitPlaneConfig GetFitPlane(int planeID) => _planes[planeID];
        public FitsConnectionZone GetLandingPins(BlockFaceDirection face)
        {
            // planes always contain pins
            var elements = new List<FitElement>();
            foreach (var plane in _planes)
            {
                if (plane.Face == face)
                {
                    var pinPositions = plane.GetFitElementsPositions();
                    var pinType = plane.FitType;
                    foreach (var pinPos in pinPositions)
                    {
                        elements.Add(new FitElement(pinType, pinPos));
                    }
                }
            }
            return new FitsConnectionZone(elements);
        }
    }
}
