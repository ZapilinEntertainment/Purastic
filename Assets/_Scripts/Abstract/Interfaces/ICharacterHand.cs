using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface ICharacterHand
	{
		public Transform HandPoint { get; }
	}
}
