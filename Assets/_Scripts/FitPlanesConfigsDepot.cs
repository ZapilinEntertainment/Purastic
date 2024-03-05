using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public static class FitPlanesConfigsDepot
	{
		private static Dictionary<int, IFitPlane> _fitPlanesList = new();

		public static int SaveConfig(IFitPlane container)
		{
			int id = container.GetHashCode();
			_fitPlanesList.TryAdd(id, container);
			return id;
		}
		public static IFitPlane LoadConfig(int hash) => _fitPlanesList[hash];
	}
}
