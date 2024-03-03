using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface ICachableModel
	{
		public GameObject ModelObject { get; }
	}
}
