using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace ZE.ServiceLocator
{
    [DefaultExecutionOrder(-100)]
    public class ServiceLocatorObject : MonoBehaviour
    {
        [SerializeField] private InstallerBase[] _installers;
        private bool _isPrepared = false;
        private int _activeContainerId = 0, _nextContainerId = 1;
        private List<Container> _containers ;
        public Container Container => _containers[_activeContainerId];
        private static ServiceLocatorObject _instance;
        public static ServiceLocatorObject Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("ServiceLocator").AddComponent<ServiceLocatorObject>();
                    _instance.Prepare();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            else
            {
                _instance = this;
                Prepare();
            }
        }
        private void Prepare()
        {
            if (!_isPrepared)
            {
                _containers = new List<Container>() { new Container(0) };
                if (_installers != null && _installers.Length > 0)
                {
                    foreach (var installer in _installers)
                    {
                        installer.Install(Container);
                    }
                }
                _isPrepared = true;
            }
        }

        public Container GetContainer(int id) => _containers[id];
        public static void GetWhenLinkReady<T>(Action<T> resolveAction, int containerID = 0) => Instance._containers[containerID].GetWhenLinkReady(resolveAction);
        public static Awaitable<T> GetWhenLinkReady<T>(int containerID = 0) => Instance._containers[containerID].GetWhenLinkReady<T>();
        public static bool TryGet<T>(out T link, int containerID = 0) => Instance._containers[containerID].TryGet<T>(out link);
        public static T Get<T>(int containerID = 0) => Instance._containers[containerID].GetLink<T>();       
        public static T GetFactory<T>(int containerID) where T : IFactory
        {
            return Instance._containers[containerID].GetFactory<T>();
        }
        public static T GetFactory<T>() where T : IFactory
        {
            return Instance.Container.GetFactory<T>();
        }
        public static LocatorLinkWrapper<T> GetLinkWrapper<T>(int containerID = 0) => Instance._containers[containerID].GetLinkWrapper<T>();
    }
}



