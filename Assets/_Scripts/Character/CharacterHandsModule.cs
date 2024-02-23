using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class CharacterHandsModule
	{		
		private IEquippable _equipment;
        private readonly Transform _handPoint;
		public System.Action<IEquippable> OnItemEquippedInHandEvent;

		public CharacterHandsModule(Transform handPoint)
		{
			_handPoint = handPoint;
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
			_equipment.OnEquip(_handPoint);
			OnItemEquippedInHandEvent?.Invoke(equippable);
			return true;
		}
	}
}
