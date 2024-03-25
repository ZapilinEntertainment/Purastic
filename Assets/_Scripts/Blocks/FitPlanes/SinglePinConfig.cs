using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public sealed class SinglePinConfig : IFitPlaneConfiguration
    {
        public readonly FitType FitType;
        public Vector2 SolePointPos => 0.5f *GameConstants.BLOCK_SIZE * Vector2.one;

        public SinglePinConfig(FitType fitType)
        {
            this.FitType = fitType;
        }

        public IContactPlaneController CreateContactPlaneController(byte planeID, BlockFaceDirection face) => new SinglePinContactPlaneController(planeID);
        public FitElementPlanePosition[] GetAllPinsInPlaneSpace() => new FitElementPlanePosition[1] {new FitElementPlanePosition(Vector2Byte.zero, SolePointPos)};
        public Vector2Byte GetFitIndex(Vector2 planedPos) => Vector2Byte.zero;
        public Vector2 GetPlanePoint(Vector2Byte index) => SolePointPos;
        public Vector3 GetZeroPos(float height) => Utilities.DefineHorizontalPlaneZeroPos(1,1,height);
        public IFitPlaneDataProvider ToDataProvider(PlaneProviderPosition position) => new SinglePinDataProvider(this, position);
        public Rect ToRect(Vector2 zeroPos) => new (zeroPos.x, zeroPos.y, GameConstants.BLOCK_SIZE, GameConstants.BLOCK_SIZE);
        public bool TryGetPlanePoint(Vector2Byte index, out Vector2 pos)
        {
            if (index == Vector2Byte.zero) {
                pos = SolePointPos;
                return true;
            }
            else
            {
                pos = Vector2.zero;return false;
            }
        }
    }
    public class SinglePinContactPlaneController : IContactPlaneController
    {
        private readonly byte _planeID;
        public SinglePinContactPlaneController(byte planeID)
        {
            _planeID = planeID;
        }

        public FitElementPlaneAddress GetContactPinAddress() => new (_planeID, Vector2Byte.zero);
        public void SetContactPin(Vector2Int index) { }
        public void Move(Vector3 localSpaceDir) { }
    }
    public class SinglePinDataProvider : FitPlaneBaseDataProvider
    {
        private FitElementPlaneAddress SolePlaneAddress => new(_position.BlockSubplaneID, Vector2Byte.zero);
        private readonly SinglePinConfig _config;
        protected override FitType FitType => _config.FitType;
        protected override Vector2 Size => GameConstants.BLOCK_SIZE * Vector2.one;

        public SinglePinDataProvider(SinglePinConfig config, PlaneProviderPosition position) : base(position)
        {
            _config = config;
        }



        protected override Vector2 IndexToPlanePosition(Vector2Byte index) => _config.SolePointPos;
        public override IReadOnlyCollection<ConnectingPin> GetAllPins() => new ConnectingPin[1] { ToConnectingPin(_config.SolePointPos, Vector2Byte.zero) };       

        public override IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle rect)
        {
            if (rect.Contains(PlaneToCutPlanePos(_config.SolePointPos))) return new ConnectingPin[1] { ToConnectingPin(_config.SolePointPos, Vector2Byte.zero) };
            else return new ConnectingPin[0];
        }
        public override FitElementPlaneAddress ToPinIndex(Vector2 cutPlanePosition) => SolePlaneAddress;

        public override bool TryGetPinPosition(Vector2 cutPlanePosition, out FitElementPlaneAddress pinPosition)
        {
            if (Contains(cutPlanePosition))
            {
                pinPosition = SolePlaneAddress; return true;
            }
            else
            {
                pinPosition = default; return false;
            }
        }
    }
}
