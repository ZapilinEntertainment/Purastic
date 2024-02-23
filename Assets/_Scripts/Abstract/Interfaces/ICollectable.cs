using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface ICollectable
	{
		public bool IsConsumable { get; }
		public bool TryGetEquippable(out IEquippable equippable);
	}
}
