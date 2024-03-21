using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PlacingBlockModelHandler
	{
        public IPlaceable Model { get; private set; }
        private MaterialsDepot _materialsDepot;
        private BlockPositionStatus _modelStatus = BlockPositionStatus.Undefined;
        private LocatorLinkWrapper<BlockModelPoolService> _modelCacheServiceWrapper;

        public bool IsReady => _modelCacheServiceWrapper.CanBeResolved;

        public PlacingBlockModelHandler()
        {
            _materialsDepot = ServiceLocatorObject.Get<MaterialsDepot>();
            _modelCacheServiceWrapper = ServiceLocatorObject.GetLinkWrapper<BlockModelPoolService>();
        }

        public void SetVisibility(bool x) => Model.IsVisible = x;
        public void ReplaceModel(IPlaceable model)
        {
            Model = model;
            ChangeModelMaterial();
        }
        public void SetModelStatus(BlockPositionStatus status)
        {
            if (_modelStatus != status)
            {
                _modelStatus = status;
                ChangeModelMaterial();
            }
        }
        private void ChangeModelMaterial()
        {
            Model?.SetDrawMaterial(_materialsDepot.GetPlacingBlockMaterial(_modelStatus));
        }
        public void Dispose()
        {
            if (Model != null)
            {
                if (Model is IPoolableModel) _modelCacheServiceWrapper.Value.CacheModel(Model as IPoolableModel);
                else
                {
                    Model.Dispose();
                }
                Model = null;
            }
        }
    }
}
