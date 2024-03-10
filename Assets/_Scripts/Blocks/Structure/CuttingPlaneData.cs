using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface ICuttingPlane
    {
        public int ID { get; }
        public BlockFaceDirection Face { get; }
        public float Coordinate { get; }

        public ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider);
        public bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider dataProvider);
        public Vector2 LocalToCutPlanePos(Vector3 localPos);
        public Vector3 CutPlaneToLocalPos(Vector2 inPlanePos);
        public Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address);
        public Vector3 PlaneAddressToLocalPos(FitElementStructureAddress structureAddress);
        public FitsConnectionZone GetLandingPinsList(AngledRectangle rect);
    }

    // contains one or more fitplanes on one face of a block

    public readonly struct CuttingPlaneCoordinate
    {
        public readonly BlockFaceDirection Direction;
        public readonly float Coordinate;

        public CuttingPlaneCoordinate(BlockFaceDirection direction, float coordinate)
        {
            Direction = direction;
            Coordinate = coordinate;
        }
        public override string ToString() => $"{Direction} :{Coordinate}";
    }

    // make a lock zone on a cutting plane, to prevent connections in it
    public class CuttingPlaneLockZone
    {
        public readonly int CuttingPlaneID;
        public readonly List<FitElementPlaneAddress> LockedElements;

        public CuttingPlaneLockZone(int planeID)
        {
            CuttingPlaneID = planeID;
            LockedElements = new ();
        }
        public CuttingPlaneLockZone(int id,IReadOnlyCollection<FitElementPlaneAddress> pinsList)
        {
            CuttingPlaneID = id;
            LockedElements = new(pinsList);
        }

        public void AddLockedPin(FitElementPlaneAddress address)
        {
            LockedElements.Add(address);
        }
        public void AddLockedPins(IReadOnlyCollection<FitElementPlaneAddress> addresses)
        {
            LockedElements.AddRange(addresses);
        }
    }
    public abstract class CuttingPlaneBase : ICuttingPlane
    {
        protected readonly int _id;
        protected readonly float _coordinate;
        protected readonly BlockFaceDirection _direction;
        public BlockFaceDirection Face => _direction;
        public float Coordinate => _coordinate;
        public int ID => _id;

        public CuttingPlaneBase(int id, BlockFaceDirection direction, float coordinate)
        {
            _id = id;
            _coordinate = coordinate;
            _direction = direction;
        }

        abstract public ICuttingPlane AddFitPlaneProvider(IFitPlaneDataProvider provider);
        abstract public bool TryDefineFitPlane(Vector2 pos, out IFitPlaneDataProvider fitPlane);
        public Vector3 CutPlaneToLocalPos(Vector2 inPlanePos) => Face.ToRotation() * new Vector3(inPlanePos.x, 0f, inPlanePos.y);
        public Vector3 PlaneAddressToLocalPos(FitElementStructureAddress structureAddress) => CutPlaneToLocalPos(PlaneAddressToCutPlanePos(structureAddress.PlaneAddress));
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
        public CuttingPlanePlaceholder(int id, CuttingPlaneCoordinate coordinate) : base(id, coordinate.Direction, coordinate.Coordinate)
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
        public override FitsConnectionZone GetLandingPinsList(AngledRectangle rect) => new (ID, _provider.GetPinsInZone(rect));
        public override Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address) =>_provider.PlaneAddressToCutPlanePosition(address);
    }
    public class ComplexCuttingPlane : CuttingPlaneBase
    {
        private readonly List<IFitPlaneDataProvider> _providers = new();
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
            return new FitsConnectionZone(ID, list);
        }
        public override Vector2 PlaneAddressToCutPlanePos(FitElementPlaneAddress address)
        {
            int id = address.SubPlaneId;
            if (id > _providers.Count) id = 0;
            return _providers[id].PlaneAddressToCutPlanePosition(address);
        }
    }
}
