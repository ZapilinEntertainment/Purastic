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
        private readonly int _blockID;
        private readonly byte _subPlaneId;
        private readonly Vector2 _cutPlaneZeroPos;
        private readonly FitsGridConfig _grid;
        private readonly PlaneOrths _cutPlaneOrths; // orths in cut plane
        private float Width => _grid.Width * GameConstants.BLOCK_SIZE;
        private float Length => _grid.Length * GameConstants.BLOCK_SIZE;
        private float ElementWidth => GameConstants.KNOB_SCALE * GameConstants.BLOCK_SIZE;
        private float ElementLength => ElementWidth;
        public PlaneOrths Orths => _cutPlaneOrths;

        public GridDataProvider(int blockID, byte subplaneId, FitsGridConfig grid, Vector2 cutPlaneZeroPos, PlaneOrths cutPlaneOrths)
        {
            _blockID = blockID;
            _subPlaneId = subplaneId;
            _cutPlaneZeroPos = cutPlaneZeroPos; _grid = grid;
            _cutPlaneOrths= cutPlaneOrths;
        }

        private Vector2 PlanePositionToCutPlanePosition(Vector2 planePos) => _cutPlaneZeroPos + _cutPlaneOrths.Right * planePos.x + _cutPlaneOrths.Up * planePos.y;
        private Vector2 CutPlanePositionToPlanePosition(Vector2 cutPlanePos)
        {
            Vector2 dir = cutPlanePos - _cutPlaneZeroPos;
            return _cutPlaneOrths.RebaseVector(dir);
        }

        public bool Contains(Vector2 pos) => ToRectangle().Contains(pos);
        public FitElementPlaneAddress ToPinIndex(Vector2 pos)
        {
            Vector2 dir = CutPlanePositionToPlanePosition(pos);
            return ToPlanePinPosition(dir.x / ElementWidth, dir.y / ElementLength);
        }
        public bool TryGetPinPosition(Vector2 cutPlanePosition, out FitElementPlaneAddress index)
        {
            Vector2 planePos = CutPlanePositionToPlanePosition(cutPlanePosition);
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
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition) => PlanePositionToCutPlanePosition(FitsGridConfig.IndexToPosition(pinPosition.PinIndex));
        private FitElementPlaneAddress ToPlanePinPosition(float x, float y) => new (0, new Vector2Byte(x, y));

        public AngledRectangle ToRectangle() => new (_cutPlaneZeroPos, _grid.ToSize(), _cutPlaneOrths);
        public VirtualPoint GetFitElementFaceVirtualPoint(Vector2Byte index) => new (PlanePositionToCutPlanePosition(FitsGridConfig.IndexToPosition(index)), _cutPlaneOrths.Quaternion);
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle cutPlaneRect)
        {
            var newRect = cutPlaneRect.ToPlaneSpace(_cutPlaneZeroPos, _cutPlaneOrths);
            //Debug.Log($"at {_cutPlaneZeroPos}x{_cutPlaneOrths}, {cutPlaneRect} -> {newRect}");
            var positions =  _grid.GetPinsInZone(newRect);

            var pins = new ConnectingPin[positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                pins[i] = ToConnectingPin(positions[i]);
            }
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
                    _blockID,
                    new FitElement(_grid.FitType, PlanePositionToCutPlanePosition(planePos.PlanePosition)),
                    new FitElementPlaneAddress(_subPlaneId, planePos.Index)
                    );
    }
}
