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
		private BlockCastModule _castModule;
		private PlacingBlockModelController _modelController;
		
		private BlockCreateService BlockCreateService => _resolver.Item2;
		public InputController InputController => _resolver.Item1;
		public BlockFaceDirection ConnectFace => GameConstants.DefaultPlacingFace;
		public PlacedBlockRotation Rotation => PlacedBlockRotation.NoRotation;
		private IBlockPlacer _blockPlacer;
		private ComplexResolver<InputController,  BlockCreateService> _resolver;

		public System.Action<PlaceableModelStatus> OnPlacementStatusChangedEvent;

        private void Awake()
        {
			_castMask = LayerConstants.GetCustomLayermask(CustomLayermask.BlockPlaceCast);
        }
        public void Start() {

			_castModule = new();
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
            if (_resolver.AllDependenciesCompleted && _castModule.IsReady)
			{
				if (_castModule.Cast(out FoundedFitElementPosition position, out RaycastHit hit))
				{
                    _placeHandler.OnPinplaneHit(new BlocksCastResult(position));
                    ChangePlacingStatus(_placeHandler.IsPlacingAllowed ? PlaceableModelStatus.CanBePlaced : PlaceableModelStatus.CannotBePlaced);
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
