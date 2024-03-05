using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class FitPlanesConfigsDepot
	{
		private static Dictionary<int, FitPlanesList> _fitPlanes = new();

		public static int SaveConfig(FitPlanesList container)
		{
			int id = container.GetHashCode();
			_fitPlanes.TryAdd(id, container);
			return id;
		}
		public static FitPlanesList LoadConfig(int hash) => _fitPlanes[hash];
	}
}
