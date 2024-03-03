using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.ServiceLocator
{
    public abstract class InstallerBase : MonoBehaviour
    {
        protected Container _container { get; private set; }
        public void Install(Container container)
        {
            _container= container;
            OnInstall();
        }
        protected abstract void OnInstall();

        protected void RegisterMonoComponentInstruction<T>(bool instantInitialize = false) where T : MonoBehaviour => _container.RegisterInstallInstruction<T>(CreateComponent<T>, instantInitialize : instantInitialize);
        protected void RegisterInstruction<T>(bool instantInitialize = false) => _container.RegisterInstallInstruction<T>(CreateObject<T>, instantInitialize: instantInitialize);
        protected void RegisterInstance<T>(T instance, bool overrideIfPresented = false) => _container.RegisterInstance(instance, writeOverIfPresented: overrideIfPresented);


        protected T CreateComponent<T>() where T : MonoBehaviour
        {
            var component =  new GameObject(typeof(T).Name).AddComponent<T>();
            if (component is IContainable) (component as IContainable).ContainerID = _container.ID;
            return component;
        }
        protected object CreateObject<T>() 
        {
            var instance =  System.Activator.CreateInstance<T>();
            if (instance is IContainable) (instance as IContainable).ContainerID = _container.ID;
            return instance;
        }
    }
}
