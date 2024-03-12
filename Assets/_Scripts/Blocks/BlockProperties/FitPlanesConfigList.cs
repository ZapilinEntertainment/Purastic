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
        public IList<FitPlaneConfig> Planes => _planes;

        public FitPlanesConfigList(FitPlaneConfig plane)
        {
            _planes = new FitPlaneConfig[1] {plane};
        }
        public FitPlanesConfigList(FitPlaneConfig[] planes)
        {
            _planes = planes;
        }
        public FitPlaneConfig GetFitPlane(int subPlaneID) => _planes[subPlaneID];
        public FitElementFacePosition GetFaceSpacePosition(Vector2Byte index, BlockFaceDirection face)
        {
            //return the pin on first suitable plane
            for (byte i = 0; i < _planes.Length; i++)
            {
                var plane = _planes[i];
                if (plane.Face == face && plane.TryGetFaceSpacePosition(index, out var pos))
                {
                    return new FitElementFacePosition(i, pos);
                }
            }
            return default;
        }
        public FitsConnectionZone CreateLandingPinsList(VirtualBlock block, BlockFaceDirection face, AngledRectangle zone, ICuttingPlane cuttingPlane)
        {
            // planes always contain pins
            var elements = new List<ConnectingPin>();
            for (byte i = 0; i < _planes.Length; i++) { 
                var plane = _planes[i];

                if (plane.Face == face)
                {
                    var pinPositions = plane.CreateDataProvider(i, block, cuttingPlane.Face).GetPinsInZone(zone);
                    foreach (var pinPos in pinPositions)
                    {
                        elements.Add(pinPos);
                    }
                }
            }
            return new FitsConnectionZone(cuttingPlane.ID, elements);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode(); 
            foreach (var element in _planes) { hash.Add(element.GetHashCode()); }
            return hash.ToHashCode();
        }
    }
}
