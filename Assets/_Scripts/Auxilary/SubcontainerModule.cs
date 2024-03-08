using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class SubcontainerModule
	{
		public SubcontainerModule(Container container) {
			container.RegisterInstance(this);
		}
	}
}
