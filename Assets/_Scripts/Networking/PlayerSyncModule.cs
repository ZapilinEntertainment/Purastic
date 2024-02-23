using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
	public struct PlayerSyncModule : INetworkSerializable
	{
        public InputController.Synchronizer InputSync;
        public PlayableCharacter.Synchronizer CharacterSync;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            InputSync.NetworkSerialize(serializer);
            CharacterSync.NetworkSerialize(serializer);
        }
    }
}
