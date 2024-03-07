using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public enum PlaceableModelStatus : byte { NotSelected, Hidden, CannotBePlaced, CanBePlaced }

    public sealed class BlockPlaceSystem : MonoBehaviour
	{
        private int _castMask;
        private PlaceableModelStatus _placementStatus = PlaceableModelStatus.NotSelected;
        private BlockPlaceHandler _placeHandler;
		private PlacingBlockModelController _modelController;
		
		private InputController InputController => _resolver.Item2;
		private CameraController CameraController => _resolver.Item1;
		private BlockCreateService BlockCreateService => _resolver.Item3;
		private IBlockPlacer _blockPlacer;
		private ComplexResolver<CameraController, InputController,  BlockCreateService> _resolver;

		public System.Action<PlaceableModelStatus> OnPlacementStatusChangedEvent;

        private void Awake()
        {
			_castMask = LayerConstants.GetCustomLayermask(CustomLayermask.BlockPlaceCast);
        }
        public void Start() {
			ChangePlacingStatus(PlaceableModelStatus.NotSelected);
			_resolver = new (OnDependenciesResolved);
			_resolver.CheckDependencies();			

            var signalBus = ServiceLocatorObject.Get<SignalBus>();
			signalBus.SubscribeToSignal<ActivateBlockPlaceSystemSignal>(OnPlaceSystemActivated);
			signalBus.SubscribeToSignal<DeactivateBlockPlaceSystemSignal>(OnPlaceSystemDeactivated);
		}
		private void OnDependenciesResolved()
		{ 		
            _placeHandler = new(ServiceLocatorObject.Get<ColliderListSystem>());
            _modelController = new PlacingBlockModelController(this, _placeHandler);

            InputController.SubscribeToKeyEvents(ControlButtonID.PlaceBlockButton, TryPlaceBlock);
        }
		private void ChangePlacingStatus(PlaceableModelStatus status)
		{
            _placementStatus = status;
			enabled = status != PlaceableModelStatus.NotSelected;

			OnPlacementStatusChangedEvent?.Invoke(status);
        }

		private async void OnPlaceSystemActivated(ActivateBlockPlaceSystemSignal signal)
		{
			_blockPlacer = signal.BlockPlacer;
			var equippedModel = _blockPlacer.GetPlaceableModel();
			var model = await BlockCreateService.CreateBlockModel(equippedModel.GetBlockProperty());

            if (model != null)
            {
                _modelController.SetModel(model);
				ChangePlacingStatus(PlaceableModelStatus.Hidden);
                enabled = true;
            }
            else
            {
                ChangePlacingStatus(PlaceableModelStatus.NotSelected);
            }
            

		}
		private void OnPlaceSystemDeactivated() => ChangePlacingStatus(PlaceableModelStatus.NotSelected);
		private void TryPlaceBlock()
		{
			if (enabled && _placementStatus == PlaceableModelStatus.CanBePlaced)
			{
				if (_placeHandler.TryAddDetail(_modelController.PlacingBlockInfo))
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
					_placeHandler.OnPinplaneHit(raycastHit);
					ChangePlacingStatus(PlaceableModelStatus.CanBePlaced);
					// todo: check for placement here
				}
				else
				{
                    ChangePlacingStatus(PlaceableModelStatus.Hidden);
                }
			}
        }
        private void Update()
        {
            if (_resolver.AllDependenciesCompleted) _modelController.UpdatePosition();
        }
    }
}
