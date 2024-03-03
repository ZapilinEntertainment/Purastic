using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZE.ServiceLocator
{
    public interface IFactory { }

    // create and register object instance
    public abstract class ContainerSingletonObjectFactory<T> : IFactory where T: IContainable
    {
        public readonly Container _container;
        public ContainerSingletonObjectFactory(Container container) => _container = container;
        public T Create()
        {
            var instance = Instantiate();
            instance.ContainerID= _container.ID;
            _container.RegisterInstance(instance); // <--- register as singleton
            return instance;
        }
        protected abstract T Instantiate();       
    }
    //simple object creation
    public abstract class ContainerObjectFactory<T> : IFactory where T : IContainable
    {
        public readonly Container _container;
        public ContainerObjectFactory(Container container) => _container = container;
        public T Create()
        {
            var instance = Instantiate();
            instance.ContainerID = _container.ID;
            return instance;
        }
        protected abstract T Instantiate();
    }
    [DefaultExecutionOrder(-100)]
    public class Container 
    {
        private Dictionary<Type, IResolveAwaiter> _awaitingItems = new ();
        private Dictionary<Type, IResolvable> _resolvablesList = new Dictionary<Type, IResolvable>();
        private Dictionary<Type, Func<object>> _installInstructions = new();
        private readonly Type _baseWrapperType = typeof(LocatorLinkWrapper<>);
        public readonly int ID;

        public Container(int id) => ID = id;
        private IResolvable CreateResolver<T>()
        {
            return (IResolvable)Activator.CreateInstance(_baseWrapperType.MakeGenericType(typeof(T)));
        }
        private IResolvable CreateResolver<T>(T instance)
        {
            return (IResolvable)Activator.CreateInstance(_baseWrapperType.MakeGenericType(typeof(T)), instance);
        }

        public void RegisterInstance<T>(T instance, bool writeOverIfPresented = true)
        {
            Type key = typeof(T);
            if (!_resolvablesList.ContainsKey(key))
            {
                _resolvablesList.Add(key, CreateResolver(instance));
            }
            else
            {
                if (writeOverIfPresented) { (_resolvablesList[key] as ILinkWrapper).SetLink(instance); }
            }
        }
        public void RegisterInstallInstruction<T>(Func<object> createFunc, bool writeOverIfPresented = true, bool instantInitialize = false)
        {
            Type key = typeof(T);
            if (!_installInstructions.ContainsKey(key) || writeOverIfPresented)
            {
                _installInstructions.Add(key, createFunc);
                if (_resolvablesList.TryGetValue(key, out var resolvable) && !resolvable.CanBeResolved)
                {                    
                    (resolvable as LocatorLinkWrapper<T>).SetLink(createFunc);
                }
            }
            if (instantInitialize) GetLinkWrapper<T>();
        }

        public void GetWhenLinkReady<T>(Action<T> resolveAction)
        {
            var wrapper = GetLinkWrapper<T>();
            if (wrapper.CanBeResolved) resolveAction.Invoke(wrapper.Value);
            else
            {
                if (_awaitingItems.TryGetValue(typeof(T), out var awaiter))
                {
                    var resolver = (ResolveAwaiter<T>)awaiter;
                    resolver.Callback += resolveAction;
                }
                else
                {
                    new ResolveAwaiter<T>(this, wrapper, resolveAction);
                }
            }
        }
        public async Awaitable<T> GetWhenLinkReady<T>()
        {
            var wrapper = GetLinkWrapper<T>();
            if (wrapper.CanBeResolved) return (T)wrapper.ValueLink;
            else
            {
                var awaiter = new AwaitableCompletionSource<T>();
                wrapper.OnValueSetEvent += OnLinkReady;
                await awaiter.Awaitable;                
                wrapper.OnValueSetEvent -= OnLinkReady;

                return wrapper.Value;

                void OnLinkReady()
                {                    
                    awaiter.SetResult(wrapper.Value);
                }
            }

            
        }

        /// <summary>
        /// observer will not be generated
        /// </summary>
        public bool IsInstancePresented<T>()
        {
            if (_resolvablesList.TryGetValue(typeof(T), out var resolver))
            {
                return resolver.CanBeResolved;
            }
            else return false;
        }
        /// <summary>
        /// observer will not be generated
        /// </summary>
        public bool TryGet<T>(out T link)
        {
            if (_resolvablesList.TryGetValue(typeof(T), out var resolver))
            {
                if (resolver.CanBeResolved)
                {
                    link = (T)resolver.ValueLink;
                    return true;
                }
            }
            link = default;
            return false;
        }
        /// <summary>
        /// observer will be generated
        /// </summary>
        public T GetLink<T>() => GetLinkWrapper<T>().Value;
        public T GetFactory<T>() where T :IFactory
        {
            if (_resolvablesList.TryGetValue(typeof(T), out IResolvable factoryWrapper))
            {
                if (factoryWrapper.CanBeResolved) return (T)factoryWrapper.ValueLink;
            }
            T factory = (T)Activator.CreateInstance(typeof(T), this);
            RegisterInstance(factory);
            return factory;
        }
        public LocatorLinkWrapper<T> GetLinkWrapper<T>()
        {
            Type key = typeof(T);
            LocatorLinkWrapper<T> resolver;
            if (_resolvablesList.TryGetValue(key, out var resolvable))
            {
                resolver = (LocatorLinkWrapper<T>)resolvable;
            }
            else
            {
                resolver = (LocatorLinkWrapper<T>)CreateResolver<T>();
                if (_installInstructions.TryGetValue(key, out var createAction))
                {                    
                    resolver.SetLink(createAction());
                }
                _resolvablesList.Add(key, resolver);
            }
            return resolver;
        }              

        private class ResolveAwaiter<T> : IResolveAwaiter
        {
            private readonly Container _container;
            private readonly LocatorLinkWrapper<T> _wrapper;
            public Action<T> Callback;

            public ResolveAwaiter(Container container, LocatorLinkWrapper<T> wrapper, Action<T> callback)
            {
                _container = container;
                _wrapper = wrapper;
                Callback = callback;

                _container._awaitingItems.Add(typeof(T), this);
                _wrapper.OnValueSetEvent += OnReadyToResolve;
            }

            public void OnReadyToResolve()
            {
                Callback?.Invoke(_wrapper.Value);
                _wrapper.OnValueSetEvent -= OnReadyToResolve;
                _container._awaitingItems.Remove(typeof(T));
            }
        }

        private interface IResolveAwaiter
        {

        }
    }
}
