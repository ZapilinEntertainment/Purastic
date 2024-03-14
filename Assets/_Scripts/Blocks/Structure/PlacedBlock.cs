using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class PlacedBlock : VirtualBlock // must be class, not struct
    {        
        public readonly int ID;
        public PlacedBlock(int id, Vector3 localPos, PlacingBlockInfo info) : base(localPos,info)
        {
            this.ID = id;
        }
        public PlacedBlock(int id, VirtualBlock virtualBlock) : base (virtualBlock)
        {
            this.ID = id;
        }

        public IReadOnlyCollection<ConnectingPin> GetAllConnectionPins(BlockFaceDirection face)
        {
            List<ConnectingPin> list = new();
            var planes = Properties.GetPlanesList().Planes;
            for (byte planeID = 0; planeID < planes.Count; planeID++)
            {
                var plane = planes[planeID];
                if (plane.Face == face)
                {
                    var pins = plane.PinsConfiguration.GetAllPinsInPlaneSpace();
                    foreach (var pin in pins)
                    {
                        list.Add(new ConnectingPin(ID, new FitElement(plane.FitType, pin.PlanePosition), new FitElementPlaneAddress(planeID, pin.Index)));
                    }
                }
            }
            return list;
        }
    }
}
