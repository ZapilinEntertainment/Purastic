using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface ICuttingPlane
    {
        public BlockFaceDirection Direction { get; }
        public float Coordinate { get; }

        public ICuttingPlane AddFitPlaneProvider(IFitPlanesDataProvider provider);
        public bool TryDefineFitPlane(Vector2 pos, out IFitPlanesDataProvider fitPlane);
        public Vector3 GetLocalPos(Vector2 inPlanePos);
    }
    public interface IFitPlanesDataProvider
    {
        public bool Contains(Vector2 pos);
        public bool TryGetPinIndex(Vector2 pos, out Vector2Byte index);
        public Vector2Byte ToPinIndex(Vector2 pos);
        public Vector2 PinIndexToPosition(Vector2Byte pinIndex);
    }

    public abstract class CuttingPlaneBase : ICuttingPlane
    {
        protected readonly float _coordinate;
        protected readonly BlockFaceDirection _direction;
        public BlockFaceDirection Direction => _direction;
        public float Coordinate => _coordinate;

        public CuttingPlaneBase(BlockFaceDirection direction, float coordinate)
        {
            _coordinate = coordinate;
            _direction = direction;
        }

        abstract public ICuttingPlane AddFitPlaneProvider(IFitPlanesDataProvider provider);
        abstract public bool TryDefineFitPlane(Vector2 pos, out IFitPlanesDataProvider fitPlane);
        public Vector3 GetLocalPos(Vector2 inPlanePos)
        {
            Vector3 localPos = new Vector3(inPlanePos.x, inPlanePos.y, Coordinate);
            return Quaternion.Inverse( Direction.ToRotation()) * localPos;
        }
    }
   
    public struct GridDataProvider : IFitPlanesDataProvider
    {
        private readonly Vector2 _zeroPos;
        private readonly FitsGrid _grid;
        private readonly PlacedBlockRotation _rotation;
        private float Width => _grid.Width * GameConstants.BLOCK_SIZE;
        private float Length => _grid.Length * GameConstants.BLOCK_SIZE;
        public GridDataProvider( FitsGrid grid, Vector2 zeroPos, PlacedBlockRotation rotation)
        {
            _zeroPos = zeroPos; _grid = grid;
            _rotation = rotation;
        }
        public bool Contains(Vector2 pos)
        {
            Vector2 dir = pos - _zeroPos;
            return dir.x > 0f && dir.y > 0f && dir.x < Width && dir.y < Length;
        }
        public Vector2Byte ToPinIndex(Vector2 pos)
        {
            Vector2 dir = pos - _zeroPos;
            return new Vector2Byte(dir.x / Width, dir.y / Length);
        }
        public bool TryGetPinIndex(Vector2 pos, out Vector2Byte index)
        {
            Vector2 dir = pos - _zeroPos;
            if (dir.x > 0f && dir.y > 0f && dir.x < Width && dir.y < Length)
            {
                index = new Vector2Byte(dir.x / Width, dir.y / Length);
                return true;
            }
            else
            {
                index = default;
                return false;
            }
        }
        public Vector2 PinIndexToPosition(Vector2Byte pinIndex) => _grid.GetFitPosition(pinIndex);
    }
    public class FullGridProvider : IFitPlanesDataProvider
    {
        private readonly Vector2 _size;

        public FullGridProvider(Vector2Byte size) => _size = (Vector2)size * GameConstants.BLOCK_SIZE;
        public bool Contains(Vector2 pos) => true;
        public Vector2 PinIndexToPosition(Vector2Byte pinIndex) => FitsGrid.IndexToPosition(pinIndex);

        public Vector2Byte ToPinIndex(Vector2 pos) => new Vector2Byte(pos.x / _size.x, pos.y / _size.y);
        public bool TryGetPinIndex(Vector2 pos, out Vector2Byte index)
        {
            index = ToPinIndex(pos);
            return true;
        }
    }
    public class OneItemCuttingPlane : CuttingPlaneBase
    {
        private readonly IFitPlanesDataProvider _provider;

        public OneItemCuttingPlane(IFitPlanesDataProvider dataProvider, BlockFaceDirection direction, float coordinate) : base(direction, coordinate)
        {
            _provider = dataProvider;
        }

        public override ICuttingPlane AddFitPlaneProvider(IFitPlanesDataProvider provider)
        {
            var complexData = new ComplexCuttingPlane(Direction, Coordinate);
            complexData.AddFitPlaneProvider(_provider);
            complexData.AddFitPlaneProvider(provider);
            return complexData;
        }

        public override bool TryDefineFitPlane(Vector2 pos, out IFitPlanesDataProvider fitPlane)
        {
            if (_provider.Contains(pos))
            {
                fitPlane = _provider;
                return true;
            }
            else
            {
                fitPlane = default;
                return false;
            }
        }
    }
    public class ComplexCuttingPlane : CuttingPlaneBase
    {
        private readonly List<IFitPlanesDataProvider> _providers = new();
        public ComplexCuttingPlane(BlockFaceDirection direction, float coordinate) : base(direction, coordinate)
        {
            
        }
        public override ICuttingPlane AddFitPlaneProvider(IFitPlanesDataProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public override bool TryDefineFitPlane(Vector2 pos, out IFitPlanesDataProvider fitPlane)
        {
            foreach (var provider in _providers)
            {
                if (provider.Contains(pos))
                {
                    fitPlane = provider;
                    return true;
                }
            }
            fitPlane= default; return false;
        }
    }
}
