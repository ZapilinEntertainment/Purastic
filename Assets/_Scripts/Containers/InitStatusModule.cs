using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class InitStatusModule
	{
		public bool IsInitialized { get; private set; } = false;
		public System.Action OnInitializedEvent;

		public void OnInitialized()
		{
			IsInitialized = true;
			OnInitializedEvent?.Invoke();
		}
	}
}
