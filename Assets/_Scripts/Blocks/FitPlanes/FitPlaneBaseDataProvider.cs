using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public abstract class FitPlaneBaseDataProvider : IFitPlaneDataProvider
    {
        abstract protected FitType FitType { get; }
        abstract protected Vector2 Size { get; }
        protected readonly PlaneProviderPosition _position;

        public int BlockID => _position.BlockID;
        public PlaneOrths Orths => _position.CutPlaneSpaceOrths;

        public FitPlaneBaseDataProvider(PlaneProviderPosition position)
        {
            _position = position;
        }

        protected Vector2 PlaneToCutPlanePos(Vector2 pos) => _position.PlanePositionToCutPlanePosition(pos);
        protected abstract Vector2 IndexToPlanePosition(Vector2Byte index);
        protected ConnectingPin ToConnectingPin(FitElementPlanePosition planePos) => ToConnectingPin(planePos.PlanePosition, planePos.Index);
        protected ConnectingPin ToConnectingPin(Vector2 pos, Vector2Byte index) => new ConnectingPin(
                    _position.BlockID,
                    new FitElement(FitType, PlaneToCutPlanePos(pos)),
                    new FitElementPlaneAddress(_position.BlockSubplaneID, index)
                    );
        

        public bool Contains(Vector2 cutPlanePosition) => ToRectangle().Contains(cutPlanePosition);
        public VirtualPoint IndexToVirtualPoint(Vector2Byte index) => new(_position.PlanePositionToCutPlanePosition(IndexToPlanePosition(index)), _position.Rotation);
        public abstract IReadOnlyCollection<ConnectingPin> GetAllPins();
        public abstract IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect);
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition) => _position.PlanePositionToCutPlanePosition(IndexToPlanePosition(pinPosition.PinIndex));
        public abstract FitElementPlaneAddress ToPinIndex(Vector2 pos);
        public AngledRectangle ToRectangle() => new(_position.CutPlaneZeroPos, Size, _position.CutPlaneSpaceOrths);
        public abstract bool TryGetPinPosition(Vector2 cutPlanePosition, out FitElementPlaneAddress pinPosition);

    }
}
