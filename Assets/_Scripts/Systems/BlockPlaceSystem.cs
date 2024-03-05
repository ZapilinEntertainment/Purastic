using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public enum PlaceableModelStatus : byte { NotSelected, Hidden, CannotBePlaced, CanBePlaced }
    public sealed class BlockPlaceSystem : MonoBehaviour
	{

        private PlaceableModelStatus _placementStatus = PlaceableModelStatus.NotSelected;
        private IPlaceable _placeableModel;

        private int _castMask;
		private BlockPlaceHandler _placeHandler;
		private LocatorLinkWrapper<BlockModelPoolService> _modelCacheServiceWrapper;
		private InputController InputController => _resolver.Item2;
		private CameraController CameraController => _resolver.Item1;
		private VisualMaterialsPack MaterialsPack => _resolver.Item3;
		private BlockCreateService BlockCreateService => _resolver.Item4;
		private IBlockPlacer _blockPlacer;
		private ComplexResolver<CameraController, InputController, VisualMaterialsPack, BlockCreateService> _resolver;

        private void Awake()
        {
			_castMask = LayerConstants.GetCustomLayermask(CustomLayermask.BlockPlaceCast);
        }
        public void Start() {
            enabled = false;

			_resolver = new (OnDependenciesResolved);
			_resolver.CheckDependencies();

			_placeHandler = new(ServiceLocatorObject.Get<ColliderListSystem>());

            var signalBus = ServiceLocatorObject.Get<SignalBus>();
			signalBus.SubscribeToSignal<ActivateBlockPlaceSystemSignal>(OnPlaceSystemActivated);
			signalBus.SubscribeToSignal<DeactivateBlockPlaceSystemSignal>(OnPlaceSystemDeactivated);

			_modelCacheServiceWrapper = ServiceLocatorObject.GetLinkWrapper<BlockModelPoolService>();
		}
		private void OnDependenciesResolved()
		{
			InputController.SubscribeToKeyEvents(ControlButtonID.PlaceBlockButton, TryPlaceBlock);
		}

		private async void OnPlaceSystemActivated(ActivateBlockPlaceSystemSignal signal)
		{
			_blockPlacer = signal.BlockPlacer;
			var equippedModel = _blockPlacer.GetPlaceableModel();
			_placeableModel = await BlockCreateService.CreateBlockModel(equippedModel.GetBlockProperty());

			if (_placeableModel != null)
			{
				_placeableModel.IsVisible = false;
				_placeableModel.SetDrawMaterial(MaterialsPack.PlacingPart);
                _placementStatus = PlaceableModelStatus.Hidden;
                enabled = true;				
			}
			else
			{
				_placementStatus = PlaceableModelStatus.NotSelected;
			}
		}
		private void OnPlaceSystemDeactivated()
		{
			enabled = false;
			if (_placeableModel != null)
			{
				if (_placeableModel is IPoolableModel) _modelCacheServiceWrapper.Value.CacheModel(_placeableModel as IPoolableModel);
				else
				{
					_placeableModel.Dispose();
				}
				_placeableModel = null;
			}
		}
		private void TryPlaceBlock()
		{
			if (enabled && _placementStatus == PlaceableModelStatus.CanBePlaced)
			{
				if (_placeHandler.TryPinDetail(_placeableModel.GetBlockProperty()))
				{
					_blockPlacer.OnDetailPlaced();
				}
			}
		}

        private void FixedUpdate()
        {
            if (_resolver.AllDependenciesCompleted)
			{
				if (CameraController.TryRaycast(InputController.CursorPosition, out RaycastHit raycastHit, _castMask))
				{
					_placeableModel.IsVisible = true;
					_placeHandler.OnPinplaneHit(raycastHit);
					_placementStatus = PlaceableModelStatus.CanBePlaced;
					// todo: check for placement here
				}
				else
				{
					_placeableModel.IsVisible = false;
                    _placementStatus = PlaceableModelStatus.Hidden;
                }
			}
        }
        private void Update()
        {
            if (enabled && _placementStatus == PlaceableModelStatus.CanBePlaced)
			{
				_placeableModel.SetPlacePosition(_placeHandler.ModelPosition);
			}
        }
    }
}
