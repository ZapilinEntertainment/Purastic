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
        public GridDataProvider(FitsGridConfig grid, Vector2 cutPlaneZeroPos, Rotation2D rotation)
        {
            _cutPlaneZeroPos = cutPlaneZeroPos; _grid = grid;
            _rotation= rotation;
        }

        private Vector2 PlanePositionToCutPlanePosition(Vector2 planePos) => _cutPlaneZeroPos + _rotation.Rotate(planePos); 
        public bool Contains(Vector2 pos) => ToRectangle().Contains(pos);
        public FitElementPlaneAddress ToPinIndex(Vector2 pos)
        {
            Vector2 dir = pos - _cutPlaneZeroPos;
            return ToPlanePinPosition(dir.x / Width, dir.y / Length);
        }
        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress index)
        {
            Vector2 dir = pos - _cutPlaneZeroPos;
            if (dir.x > 0f && dir.y > 0f && dir.x < Width && dir.y < Length)
            {
                index = ToPlanePinPosition(dir.x / Width, dir.y / Length);
                return true;
            }
            else
            {
                index = default;
                return false;
            }
        }
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition) => PlanePositionToCutPlanePosition(FitsGridConfig.IndexToPosition(pinPosition.PinIndex));
        private FitElementPlaneAddress ToPlanePinPosition(float x, float y) => new FitElementPlaneAddress(0, new Vector2Byte(x, y));

        public AngledRectangle ToRectangle() => new AngledRectangle(new Rect(_cutPlaneZeroPos, _grid.ToSize()), _rotation);
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect) => _grid.GetPinsInZone(rect);
        public IReadOnlyCollection<FitElement> GetAllPins() => _grid.GetAllPins();
    }
    public class FullGridProvider : IFitPlaneDataProvider
    {
        public readonly FitType FitType;
        private readonly Vector2 _modelSize;

        public FullGridProvider(Vector2Byte size, FitType fitType)
        {
            _modelSize = (Vector2)size * GameConstants.BLOCK_SIZE;
            FitType = fitType;
        }
        public bool Contains(Vector2 pos) => true;


        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress index)
        {
            index = ToPinIndex(pos);
            return true;
        }
        public Vector2 PlaneAddressToCutPlanePosition(FitElementPlaneAddress pinPosition) => FitsGridConfig.IndexToPosition(pinPosition.PinIndex);

        public FitElementPlaneAddress ToPinIndex(Vector2 pos) => new FitElementPlaneAddress(new Vector2Byte(pos.x / _modelSize.x, pos.y / _modelSize.y));

        public AngledRectangle ToRectangle() => new (new Rect(Vector2.zero, _modelSize), Rotation2D.NoRotation);
        private FitsGridConfig ToGrid() => new FitsGridConfig(FitType, _modelSize);
        public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect) => this.ToGrid().GetPinsInZone(rect);
        public IReadOnlyCollection<FitElement> GetAllPins() => this.ToGrid().GetAllPins();
    }
}
