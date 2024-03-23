using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class GridDataProvider : FitPlaneBaseDataProvider
    {
        // contains only one grid        
        private readonly FitsGridConfig _grid;
        private float Width => _grid.Width * GameConstants.BLOCK_SIZE;
        private float Length => _grid.Length * GameConstants.BLOCK_SIZE;
        private float ElementWidth => GameConstants.KNOB_SCALE * GameConstants.BLOCK_SIZE;
        private float ElementLength => ElementWidth;
        protected override FitType FitType => _grid.FitType;
        protected override Vector2 Size => _grid.ToSize();

        public GridDataProvider(FitsGridConfig grid, PlaneProviderPosition position) : base(position) 
        {
            _grid = grid;
        }


        protected override Vector2 IndexToPlanePosition(Vector2Byte index) => FitsGridConfig.IndexToPosition(index);

        override public FitElementPlaneAddress ToPinIndex(Vector2 pos)
        {
            Vector2 dir = _position.CutPlanePositionToPlanePosition(pos);
            return ToPlanePinPosition(dir.x / ElementWidth, dir.y / ElementLength);
        }
        override public bool TryGetPinPosition(Vector2 cutPlanePosition, out FitElementPlaneAddress index)
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
        private FitElementPlaneAddress ToPlanePinPosition(float x, float y) => new (_position.BlockSubplaneID, new Vector2Byte(x, y));              
        override public IReadOnlyCollection<ConnectingPin> GetPinsInZone(AngledRectangle cutPlaneRect)
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
        override public IReadOnlyCollection<ConnectingPin> GetAllPins()
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
    }
}
