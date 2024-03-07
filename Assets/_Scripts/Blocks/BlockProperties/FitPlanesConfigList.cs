using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
    // contains all block planes info
    // 
	public class FitPlanesConfigList
	{
        private readonly FitPlaneConfig[] _planes;
        public IReadOnlyCollection<FitPlaneConfig> Planes => _planes;

        public FitPlanesConfigList(FitPlaneConfig plane)
        {
            _planes = new FitPlaneConfig[1] {plane};
        }
        public FitPlanesConfigList(FitPlaneConfig[] planes)
        {
            _planes = planes;
        }
        public FitPlaneConfig GetFitPlane(int planeID) => _planes[planeID];
        public FitsConnectionZone CreateLandingPinsList(PlacedBlock block, BlockFaceDirection face, AngledRectangle zone, int cutPlaneID)
        {
            // planes always contain pins
            var elements = new List<ConnectingPin>();
            for (int i = 0; i < _planes.Length; i++) { 
                var plane = _planes[i]; 
                if (plane.Face == face)
                {
                    var pinPositions = plane.CreateDataProvider(block).GetPinsInZone(zone);
                    var pinType = plane.FitType;
                    foreach (var pinPos in pinPositions)
                    {
                        elements.Add(pinPos);
                    }
                }
            }
            return new FitsConnectionZone(cutPlaneID, elements);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode(); 
            foreach (var element in _planes) { hash.Add(element.GetHashCode()); }
            return hash.ToHashCode();
        }
    }
}
