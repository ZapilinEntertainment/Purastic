using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
	public static class DebugOutputUtility
	{
		public static void LogNetworkBehaviourStatus(NetworkBehaviour networkBehaviour)
		{
			Debug.Log($"server: {networkBehaviour.IsServer}, host: {networkBehaviour.IsHost}, owner: {networkBehaviour.IsOwner}, client: {networkBehaviour.IsClient}");
		}
	}
}
