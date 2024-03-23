using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public interface IContactPlaneController
	{
		// used for shifting detail contact pins when placing

		public void Move(Vector3 localSpaceDir);
		public void SetContactPin(Vector2Int index);
		public FitElementPlaneAddress GetContactPinAddress();
    }
	
}
