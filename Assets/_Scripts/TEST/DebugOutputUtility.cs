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
		public static void LogObjects<T>(IReadOnlyCollection<T> list, bool writeCount = true)
		{
			var builder = new System.Text.StringBuilder();
			if (writeCount) builder.AppendLine(list.Count.ToString());
			foreach ( var item in list )
			{
				builder.AppendLine(item.ToString());
			}
			Debug.Log(builder.ToString());
		}

		public static Transform SpawnCube(Vector3 pos)
		{
			var model = GameObject.CreatePrimitive(PrimitiveType.Cube);
			var transform = model.transform;
			transform.localScale = GameConstants.BLOCK_SIZE * Vector3.one;
			transform.position = pos;
			return transform;
		}
	}
}
