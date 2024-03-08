using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface ILateInitializable
	{
		public bool IsInitialized => InitStatusModule == null || InitStatusModule.IsInitialized;
		public InitStatusModule InitStatusModule { get; }
	}
}
