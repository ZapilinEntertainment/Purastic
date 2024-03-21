using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface ICuttingPlane // must be class
    {
        public int ID { get; }
        public BlockFaceDirection Face { get; }
        public float Coordinate { get; }
        public int PlanesCount { get; }

        public ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider);
        public bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider dataProvider);
        public Vector2 LocalToCutPlanePos(Vector3 localPos);
        public Vector3 CutPlaneToLocalPos(Vector2 inPlanePos);
        public Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address);
        public Vector3 PlaneAddressToLocalPos(FitElementPlaneAddress planeAddress);
        public Vector3 PlaneAddressToLocalPos(FitElementStructureAddress structureAddress);
        public FitsConnectionZone GetLandingPinsList(AngledRectangle rect);

        public CuttingPlanePosition ToCoordinate() => new (Face, Coordinate);
        public CuttingPlanePosition GetMirrorPosition() => new(Face.Inverse(),Coordinate);
    }

    public abstract class CuttingPlaneBase : ICuttingPlane
    {
        protected readonly int _id;
        protected readonly float _coordinate;
        protected readonly BlockFaceDirection _direction;
        public BlockFaceDirection Face => _direction;
        public float Coordinate => _coordinate;
        public int ID => _id;
        public abstract int PlanesCount { get; }

        public CuttingPlaneBase(int id, BlockFaceDirection direction, float coordinate)
        {
            _id = id;
            _coordinate = coordinate;
            _direction = direction;
        }

        abstract public ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider);
        abstract public bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider fitPlane);
        public Vector3 CutPlaneToLocalPos(Vector2 inPlanePos)
        {
            Vector3 localProjectedPos = new FaceOrths(Face.ToRotation()).TransformVector(inPlanePos);
            return localProjectedPos + _coordinate * _direction.Normal;
        }
        public Vector3 PlaneAddressToLocalPos(FitElementStructureAddress structureAddress) => CutPlaneToLocalPos(PlaneAddressToCutPlanePos(structureAddress.PlaneAddress));
        public Vector3 PlaneAddressToLocalPos(FitElementPlaneAddress planeAddress) => CutPlaneToLocalPos(PlaneAddressToCutPlanePos(planeAddress));
        public Vector2 LocalToCutPlanePos(Vector3 localPos)
        {
            Vector3 projected = Vector3.ProjectOnPlane(localPos, Face.Normal);
            return Face.InverseVector(projected);
        }
        abstract public Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address);

        abstract public FitsConnectionZone GetLandingPinsList(AngledRectangle rect);
    }
   
    public class CuttingPlanePlaceholder : CuttingPlaneBase
    {
        public override int PlanesCount => 0;
        public CuttingPlanePlaceholder(int id, CuttingPlanePosition coordinate) : base(id, coordinate.Face, coordinate.Coordinate)
        {
           
        }
        public override Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address) => FitsGridConfig.IndexToPosition(address.PinIndex);
        public override ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider)
        {
            return new OneItemCuttingPlane(ID, provider, Face, Coordinate);
        }

        public override bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider fitPlane)
        {
            fitPlane = null;
            return false;
        }
        public override FitsConnectionZone GetLandingPinsList(AngledRectangle rect) => null;
    }
    public class OneItemCuttingPlane : CuttingPlaneBase
    {
        public override int PlanesCount => 1;
        private readonly IFitPlaneDataProvider _provider;

        public OneItemCuttingPlane(int id, IFitPlaneDataProvider dataProvider, BlockFaceDirection direction, float coordinate) : base(id, direction, coordinate)
        {
            _provider = dataProvider;
        }

        public override ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider)
        {
            var complexData = new ComplexCuttingPlane(ID, Face, Coordinate);
            complexData.AddFitPlaneProvider(_provider);
            complexData.AddFitPlaneProvider(provider);
            return complexData;
        }

        public override bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider dataProvider)
        {
            //Debug.Log($"pos {pos} in {_provider.ToRectangle().Rect}");
            if (_provider.Contains(pos))
            {
               
                dataProvider = _provider;
                return true;
            }
            else
            {
                dataProvider = null;
                return false;
            }
        }
        public override FitsConnectionZone GetLandingPinsList(AngledRectangle rect) => new (Face, _provider.GetPinsInZone(rect));
        public override Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address) =>_provider.PlaneAddressToCutPlanePosition(address);
    }
    public class ComplexCuttingPlane : CuttingPlaneBase
    {
        private readonly List<IFitPlaneDataProvider> _providers = new();
        public override int PlanesCount => _providers.Count;
        public ComplexCuttingPlane(int id, BlockFaceDirection direction, float coordinate) : base(id, direction, coordinate)
        {
            
        }
        public override ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public override bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider fitPlane)
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

        public override FitsConnectionZone GetLandingPinsList(AngledRectangle rect)
        {
            var list = new List<ConnectingPin>();
            foreach (var provider in _providers)
            {
                if (provider.ToRectangle().IsIntersects(rect)) list.AddRange(provider.GetPinsInZone(rect));
            }
            return new FitsConnectionZone(Face, list);
        }
        public override Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address)
        {
            int id = address.SubPlaneId;
            if (id > _providers.Count) id = 0;
            return _providers[id].PlaneAddressToCutPlanePosition(address);
        }
    }
}
