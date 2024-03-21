using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface IFitPlaneDataProvider
    {
        public PlaneOrths Orths { get; }    
        public bool Contains(Vector2 pos);
        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress pinPosition);
        public FitElementPlaneAddress ToPinIndex(Vector2 pos);
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition);
        public AngledRectangle ToRectangle();
        public VirtualPoint GetFitElementFaceVirtualPoint(Vector2Byte index);

        public IReadOnlyCollection<ConnectingPin> GetAllPins();
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect);
    }

    public readonly struct GridDataProvider : IFitPlaneDataProvider
    {
        // contains only one grid
        private readonly PlaneProviderPosition _position;
        private readonly FitsGridConfig _grid;
        private float Width => _grid.Width * GameConstants.BLOCK_SIZE;
        private float Length => _grid.Length * GameConstants.BLOCK_SIZE;
        private float ElementWidth => GameConstants.KNOB_SCALE * GameConstants.BLOCK_SIZE;
        private float ElementLength => ElementWidth;
        public PlaneOrths Orths => _position.CutPlaneSpaceOrths;

        public GridDataProvider(FitsGridConfig grid, PlaneProviderPosition position)
        {
            _grid = grid;
            _position = position;
        }       

        public bool Contains(Vector2 pos) => ToRectangle().Contains(pos);
        public FitElementPlaneAddress ToPinIndex(Vector2 pos)
        {
            Vector2 dir = _position.CutPlanePositionToPlanePosition(pos);
            return ToPlanePinPosition(dir.x / ElementWidth, dir.y / ElementLength);
        }
        public bool TryGetPinPosition(Vector2 cutPlanePosition, out FitElementPlaneAddress index)
        {
            Vector2 planePos = _position.CutPlanePositionToPlanePosition(cutPlanePosition);
            if (planePos.x > 0f && planePos.y > 0f && planePos.x < Width && planePos.y < Length)
            {
                index = ToPlanePinPosition(planePos.x / ElementWidth, planePos.y / ElementLength);
                return true;
            }
            else
            {
                index = default;
                return false;
            }
        }
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition) => _position.PlanePositionToCutPlanePosition(FitsGridConfig.IndexToPosition(pinPosition.PinIndex));
        private FitElementPlaneAddress ToPlanePinPosition(float x, float y) => new (0, new Vector2Byte(x, y));

        public AngledRectangle ToRectangle() => new (_position.CutPlaneZeroPos, _grid.ToSize(), _position.CutPlaneSpaceOrths);
        public VirtualPoint GetFitElementFaceVirtualPoint(Vector2Byte index) => new (_position.PlanePositionToCutPlanePosition(FitsGridConfig.IndexToPosition(index)), _position.Rotation);
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle cutPlaneRect)
        {
            var newRect = cutPlaneRect.ToPlaneSpace(_position.CutPlaneZeroPos, _position.CutPlaneSpaceOrths);
            //Debug.Log($"at {_position.CutPlaneZeroPos}x{_position.CutPlaneSpaceOrths}, {cutPlaneRect} -> {newRect}");
            var positions =  _grid.GetPinsInZone(newRect);

            var pins = new ConnectingPin[positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                pins[i] = ToConnectingPin(positions[i]);
            }
            //Debug.Log($"{_position.BlockID}x{_grid.FitType}");
            return pins;

        }
        public IReadOnlyCollection<ConnectingPin> GetAllPins()
        {
            var pins = _grid.GetAllPinsInPlaneSpace();
            int count = pins.Length;
            var connectionPins = new ConnectingPin[count];
            for (int i = 0;i < count; i++)
            {
                connectionPins[i] = ToConnectingPin(pins[i]);
            }            
            return connectionPins;
        }
        private ConnectingPin ToConnectingPin(FitElementPlanePosition planePos) => new ConnectingPin(
                    _position.BlockID,
                    new FitElement(_grid.FitType, _position.PlanePositionToCutPlanePosition(planePos.PlanePosition)),
                    new FitElementPlaneAddress(_position.BlockSubplaneID, planePos.Index)
                    );
    }
}
