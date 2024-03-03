using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class EquipmentModule : IBlockPlacer
    {

		private readonly bool _isOwner;
		private IEquippable _equippedItem;
		private PlayableCharacter _activeCharacter;
		private SignalBus _signalBus;
		public System.Action<IEquippable> OnItemEquippedInHandEvent;

		public EquipmentModule(ICharacterController characterController, bool isOwner)
		{
			_isOwner = isOwner;
			_signalBus = ServiceLocatorObject.Get<SignalBus>();

			characterController.OnCharacterChangedEvent += OnCharacterChanged;
			if (characterController.ActiveCharacter != null) OnCharacterChanged(characterController.ActiveCharacter);

		}

		public void OnCharacterChanged(PlayableCharacter character)
		{
			_activeCharacter = character;
		}

        public bool TryGetInHand(ICollectable collectable)
		{
			if (collectable.TryGetEquippable(out var equippable))
			{
				return TryEquip(equippable);
			}
			else return false;
		}

		public bool TryEquip(IEquippable equippable)
		{
			_equippedItem= equippable;
			_equippedItem.OnEquip(_activeCharacter.HandPoint);

			OnItemEquippedInHandEvent?.Invoke(equippable);
			if (_isOwner && _equippedItem.IsPlaceable) _signalBus.FireSignal(new ActivateBlockPlaceSystemSignal(this));
			return true;
		}


        public IPlaceable GetPlaceableModel()
        {
			if (_equippedItem.IsPlaceable)
			{
				return (_equippedItem as IPlaceable);
			}
			else return null;
        }

        public void OnDetailPlaced()
        {
            throw new System.NotImplementedException();
        }
    }
}
