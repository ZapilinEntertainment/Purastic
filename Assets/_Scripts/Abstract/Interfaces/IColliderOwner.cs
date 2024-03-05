using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {
	public interface IColliderOwner 
	{
		abstract public bool HaveMultipleColliders { get; }
		public int GetColliderID();
		public IReadOnlyCollection<int> GetColliderIDs();
	}
}
