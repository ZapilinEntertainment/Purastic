using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
	public sealed class TestModule : MonoBehaviour
	{
        [System.Serializable] public enum LaunchMode : byte { Host,Client,Server, NoActions}
        [SerializeField] private LaunchMode _launchMode = LaunchMode.NoActions;
        private void Start()
        {
            switch (_launchMode) {
                case LaunchMode.Host:
                    ServiceLocatorObject.Get<NetworkManager>().StartHost();
                    break;
                case LaunchMode.Client:
                    ServiceLocatorObject.Get<NetworkManager>().StartClient();
                    break;
                case LaunchMode.Server:
                    ServiceLocatorObject.Get<NetworkManager>().StartServer();
                    break;
             }
        }

        private void Update()
        {
           // if (Input.GetKeyDown("x")) ServiceLocatorObject.Instance.Container.GetLinkWrapper<CharacterCreateService>();
        }
    }
}
