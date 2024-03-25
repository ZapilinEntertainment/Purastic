using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface IFitPlaneDataProvider
    {
        public int BlockID { get; }
        public int SubplaneID { get; }
        public PlaneOrths Orths { get; }
        public bool Contains(Vector2 cutPlanePosition);
        public bool TryGetPinPosition(Vector2 cutPlanePosition, out FitElementPlaneAddress pinPosition);
        public FitElementPlaneAddress ToPinIndex(Vector2 cutPlanePosition);
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition);
        public AngledRectangle ToRectangle();
        public VirtualPoint IndexToVirtualPoint(Vector2Byte index);

        public IReadOnlyCollection<ConnectingPin> GetAllPins();
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect);
    }
}
