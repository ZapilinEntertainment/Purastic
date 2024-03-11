using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface IFitPlaneDataProvider
    {
        public bool Contains(Vector2 pos);
        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress pinPosition);
        public FitElementPlaneAddress ToPinIndex(Vector2 pos);
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition);
        public AngledRectangle ToRectangle();
        public VirtualPoint GetFitElementFaceVirtualPoint(Vector2Byte index);

        public IReadOnlyCollection<FitElement> GetAllPins();
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect);
    }

    public readonly struct GridDataProvider : IFitPlaneDataProvider
    {
        // contains only one grid
        private readonly Vector2 _cutPlaneZeroPos;
        private readonly FitsGridConfig _grid;
        private readonly Rotation2D _rotation;
        private float Width => _grid.Width * GameConstants.BLOCK_SIZE;
        private float Length => _grid.Length * GameConstants.BLOCK_SIZE;
        private float ElementWidth => GameConstants.KNOB_SCALE * GameConstants.BLOCK_SIZE;
        private float ElementLength => ElementWidth;
        public GridDataProvider(FitsGridConfig grid, Vector2 cutPlaneZeroPos, Rotation2D rotation)
        {
            _cutPlaneZeroPos = cutPlaneZeroPos; _grid = grid;
            _rotation= rotation;
        }

        private Vector2 PlanePositionToCutPlanePosition(Vector2 planePos) => _cutPlaneZeroPos + _rotation.Rotate(planePos);
        private Vector2 CutPlanePositionToPlanePosition(Vector2 cutPlanePos)
        {
            Vector2 dir = cutPlanePos - _cutPlaneZeroPos;
            return _rotation.InverseDirection(dir);
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

        public AngledRectangle ToRectangle() => new AngledRectangle(new Rect(_cutPlaneZeroPos, _grid.ToSize()), _rotation);
        public VirtualPoint GetFitElementFaceVirtualPoint(Vector2Byte index) => new (PlanePositionToCutPlanePosition(FitsGridConfig.IndexToPosition(index)), _rotation.ToQuaternion());
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect) => _grid.GetPinsInZone(rect);
        public IReadOnlyCollection<FitElement> GetAllPins() => _grid.GetAllPins();
    }
}
