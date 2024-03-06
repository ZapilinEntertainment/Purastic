using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface IFitPlanesDataProvider
    {
        public bool Contains(Vector2 pos);
        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress pinPosition);
        public FitElementPlaneAddress ToPinIndex(Vector2 pos);
        public Vector2 PinIndexToPosition(FitElementPlaneAddress pinPosition);
        public AngledRectangle ToRectangle();
        public IReadOnlyCollection<ConnectionPin> GetPinsInZone(AngledRectangle rect);
    }

    public struct GridDataProvider : IFitPlanesDataProvider
    {
        // contains only one grid
        private readonly Vector2 _zeroPos;
        private readonly FitsGridConfig _grid;
        private readonly PlacedBlockRotation _rotation;
        private float Width => _grid.Width * GameConstants.BLOCK_SIZE;
        private float Length => _grid.Length * GameConstants.BLOCK_SIZE;
        public GridDataProvider(FitsGridConfig grid, Vector2 zeroPos, PlacedBlockRotation rotation)
        {
            _zeroPos = zeroPos; _grid = grid;
            _rotation = rotation;
        }
        public bool Contains(Vector2 pos)
        {
            Vector2 dir = pos - _zeroPos;
            return dir.x > 0f && dir.y > 0f && dir.x < Width && dir.y < Length;
        }
        public FitElementPlaneAddress ToPinIndex(Vector2 pos)
        {
            Vector2 dir = pos - _zeroPos;
            return ToPlanePinPosition(dir.x / Width, dir.y / Length);
        }
        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress index)
        {
            Vector2 dir = pos - _zeroPos;
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
        public Vector2 PinIndexToPosition(FitElementPlaneAddress pinPosition) => FitsGridConfig.IndexToPosition(pinPosition.PinIndex);
        private FitElementPlaneAddress ToPlanePinPosition(float x, float y) => new FitElementPlaneAddress(0, new Vector2Byte(x, y));
    }
    public class FullGridProvider : IFitPlanesDataProvider
    {
        private readonly Vector2 _size;

        public FullGridProvider(Vector2Byte size) => _size = (Vector2)size * GameConstants.BLOCK_SIZE;
        public bool Contains(Vector2 pos) => true;
        public Vector2 PinIndexToPosition(FitElementPlaneAddress pinPosition) => FitsGridConfig.IndexToPosition(pinPosition.PinIndex);

        public FitElementPlaneAddress ToPinIndex(Vector2 pos) => new FitElementPlaneAddress(new Vector2Byte(pos.x / _size.x, pos.y / _size.y));
        public bool TryGetPinPosition(Vector2 pos, out FitElementPlaneAddress index)
        {
            index = ToPinIndex(pos);
            return true;
        }
    }
}
