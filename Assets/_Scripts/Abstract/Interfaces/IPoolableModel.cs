using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IPoolableModel : System.IDisposable
	{
        public void OnSpawnedFromPool();
        public void OnReturnedToPool();		
		public GameObject ModelObject { get; }
	}
}
