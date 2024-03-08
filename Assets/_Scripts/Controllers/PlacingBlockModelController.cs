using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class PlacingBlockModelController
    {
        private bool _isActive = false;
        private bool? _drawPlaceAllowedMaterial = null;
        private PlacedBlockRotation _placedBlockRotation;
        private IPlaceable _placeableModel;
        private Material _placingAvailableMaterial, _placingBlockedMaterial;
        private BlockPlaceHandler _placeHandler;
        private LocatorLinkWrapper<BlockModelPoolService> _modelCacheServiceWrapper;

        public PlacingBlockInfo PlacingBlockInfo => new PlacingBlockInfo(_placeableModel.GetBlockProperty(), _placedBlockRotation);

        public PlacingBlockModelController(BlockPlaceSystem blockPlaceSystem, BlockPlaceHandler placeHandler)
        {
            var materialsDepot = ServiceLocatorObject.Get<MaterialsDepot>();
            _placingAvailableMaterial = materialsDepot.GetPlacingBlockMaterial(true);
            _placingBlockedMaterial = materialsDepot.GetPlacingBlockMaterial(false);
            _modelCacheServiceWrapper = ServiceLocatorObject.GetLinkWrapper<BlockModelPoolService>();
            _placeHandler = placeHandler;

            blockPlaceSystem.OnPlacementStatusChangedEvent += OnPlacementStatusChanged;
            _placeHandler.OnPlacingPermitChangedEvent += UpdateMaterial;
        }
        private void OnPlacementStatusChanged(PlaceableModelStatus status)
        {
            switch (status)
            {
                case PlaceableModelStatus.NotSelected: Stop(); break;
                case PlaceableModelStatus.Hidden: 
                    if (_placeableModel != null) _placeableModel.IsVisible = false;
                    _isActive = false;
                    break;
                case PlaceableModelStatus.CannotBePlaced:
                case PlaceableModelStatus.CanBePlaced:
                    if (_placeableModel != null) _placeableModel.IsVisible = true;
                    _isActive = true;
                    break;
            }
        }

        public void SetModel(IPlaceable model)
        {
            _placeableModel = model;
            _placeableModel.IsVisible = false;
            UpdateMaterial(_placeHandler.IsPlacingAllowed);
        }
        private void UpdateMaterial(bool placingAllowed)
        {
            if (placingAllowed != _drawPlaceAllowedMaterial)
            {
                _drawPlaceAllowedMaterial = placingAllowed;
                _placeableModel?.SetDrawMaterial(placingAllowed ? _placingAvailableMaterial : _placingBlockedMaterial);
            }
        }
        public void Stop()
        {
            if (_placeableModel != null)
            {
                if (_placeableModel is IPoolableModel) _modelCacheServiceWrapper.Value.CacheModel(_placeableModel as IPoolableModel);
                else
                {
                    _placeableModel.Dispose();
                }
                _placeableModel = null;
            }
            _isActive = false;
        }

        public void UpdatePosition()
        {
            if (_isActive && _placeableModel != null) _placeableModel.SetPoint(_placeHandler.ModelPoint.Position, _placeHandler.ModelPoint.Rotation);
        }
    }
}
