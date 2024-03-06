using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class FitPlanesConfigsDepot
	{
		private static Dictionary<int, FitPlanesConfigList> _fitPlanes = new();

		public static int SaveConfig(FitPlanesConfigList container)
		{
			int id = container.GetHashCode();
			_fitPlanes.TryAdd(id, container);
			return id;
		}
		public static FitPlanesConfigList LoadConfig(int hash) => _fitPlanes[hash];
	}
}
