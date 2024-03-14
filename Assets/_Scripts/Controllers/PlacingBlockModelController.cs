using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class PlacingBlockModelController
    {
        private bool _isActive = false;
        private BlockPositionStatus _positionStatus = BlockPositionStatus.Undefined;
        private PlacingBlockModelHandler _model;
        private BlockPlaceHandler _placeHandler;
       
        private readonly BlockPlaceSystem _placeSystem;

        public PlacingBlockInfo PlacingBlockInfo => new (Vector2Byte.one, _model.Model.GetBlockProperty(), _placeSystem.ConnectFace, _placeSystem.Rotation);

        public PlacingBlockModelController(BlockPlaceSystem blockPlaceSystem, BlockPlaceHandler placeHandler)
        {
            _model = new();
            _placeSystem = blockPlaceSystem;            
            _placeHandler = placeHandler;

            blockPlaceSystem.OnPlacementStatusChangedEvent += OnPlacementStatusChanged;
            _placeHandler.OnPlacingPermitChangedEvent += OnPlaceStatusChanged;
        }
        private void OnPlacementStatusChanged(PlaceableModelStatus status)
        {
            switch (status)
            {
                case PlaceableModelStatus.NotSelected: Stop(); break;
                case PlaceableModelStatus.Hidden:
                    _model?.SetVisibility(false);
                    _isActive = false;
                    break;
                case PlaceableModelStatus.CannotBePlaced:
                case PlaceableModelStatus.CanBePlaced:
                    _model?.SetVisibility(true);
                    _isActive = true;
                    break;
            }
        }

        public void SetModel(IPlaceable model)
        {
            _model.ReplaceModel(model);
            _model.SetVisibility(false);
        }
        private void OnPlaceStatusChanged(BlockPositionStatus status)
        {
            _positionStatus= status;
            _model.SetModelStatus(_positionStatus);
        }
        public void Stop()
        {
            _model?.Dispose();
            _isActive = false;
        }

        public void UpdatePosition()
        {
            if (_isActive) _model.Model.SetPoint(_placeHandler.ModelPoint.Position, _placeHandler.ModelPoint.Rotation);
        }
    }
}
