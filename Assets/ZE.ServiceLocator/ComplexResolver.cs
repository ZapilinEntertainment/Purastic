using System.Collections;
using System.Collections.Generic;
using System;

namespace ZE.ServiceLocator
{
    public abstract class ComplexResolver
    {
        public bool AllDependenciesCompleted { get; private set; }
        private int _containerID = 0;
        protected IntCompleteMask _completeMask;
        protected readonly Container Container;
        public Action OnAllDependenciesResolvedEvent;

        public ComplexResolver(Action completeCallback, int containerID = 0) : this(completeCallback, ServiceLocatorObject.Instance.GetContainer(containerID)) { }
        public ComplexResolver(Action completeCallback, Container container)
        {
            OnAllDependenciesResolvedEvent += completeCallback;
            Container = container;
        }
        protected void MakeComplete()
        {
            AllDependenciesCompleted = true;
            OnAllDependenciesResolvedEvent?.Invoke();
            OnComplete();
        }
        abstract protected void OnComplete();

    }
    public class ComplexResolver<T1,T2> : ComplexResolver
    {
        public T1 Item1 => Wrapper1.Value;
        public T2 Item2 => Wrapper2.Value;
        public readonly LocatorLinkWrapper<T1> Wrapper1;
        public readonly LocatorLinkWrapper<T2> Wrapper2;
        public ComplexResolver(Action completeCallback, Container container) : base(completeCallback, container) 
        {
            _completeMask = new IntCompleteMask(2);         
            Wrapper1 = Container.GetLinkWrapper<T1>();          
            Wrapper2 = Container.GetLinkWrapper<T2>();

            Wrapper1.OnValueSetEvent += CheckDependencies;
            Wrapper2.OnValueSetEvent += CheckDependencies;
        }
        public ComplexResolver(Action completeCallback, int containerID = 0) : this(completeCallback, ServiceLocatorObject.Instance.GetContainer(containerID)) { }

        public void CheckDependencies()
        {
            bool result = false;
            if (Wrapper1.CanBeResolved) result |= _completeMask.CompleteFlag(0);
            if (Wrapper2.CanBeResolved) result |= _completeMask.CompleteFlag(1);
           // UnityEngine.Debug.Log(result);
            if (result) MakeComplete();
        }
        protected override void OnComplete()
        {
            Wrapper1.OnValueSetEvent -= CheckDependencies;
            Wrapper2.OnValueSetEvent -= CheckDependencies;
        }
    }
    public class ComplexResolver<T1, T2, T3> : ComplexResolver
    {
        public T1 Item1 => Wrapper1.Value;
        public T2 Item2 => Wrapper2.Value;
        public T3 Item3 => Wrapper3.Value;
        public readonly LocatorLinkWrapper<T1> Wrapper1;
        public readonly LocatorLinkWrapper<T2> Wrapper2;
        public readonly LocatorLinkWrapper<T3> Wrapper3;
        public ComplexResolver(Action completeCallback, Container container) : base(completeCallback, container)
        {
            _completeMask = new IntCompleteMask(3);
            Wrapper1 = Container.GetLinkWrapper<T1>();
            Wrapper2 = Container.GetLinkWrapper<T2>();
            Wrapper3 = Container.GetLinkWrapper<T3>();

            Wrapper1.OnValueSetEvent += CheckDependencies;
            Wrapper2.OnValueSetEvent += CheckDependencies;
            Wrapper3.OnValueSetEvent += CheckDependencies;
        }
        public ComplexResolver(Action completeCallback, int containerID = 0) : this(completeCallback, ServiceLocatorObject.Instance.GetContainer(containerID)) { }

        public void CheckDependencies()
        {
            bool result = false;
            if (Wrapper1.CanBeResolved) result |= _completeMask.CompleteFlag(1);
            if (Wrapper2.CanBeResolved) result |= _completeMask.CompleteFlag(2);
            if (Wrapper3.CanBeResolved) result |= _completeMask.CompleteFlag(0);
            if (result) MakeComplete();
        }
        protected override void OnComplete()
        {
            Wrapper1.OnValueSetEvent -= CheckDependencies;
            Wrapper2.OnValueSetEvent -= CheckDependencies;
            Wrapper3.OnValueSetEvent -= CheckDependencies;
        }
    }
    public class ComplexResolver<T1, T2, T3, T4> : ComplexResolver
    {
        public T1 Item1 => Wrapper1.Value;
        public T2 Item2 => Wrapper2.Value;
        public T3 Item3 => Wrapper3.Value;
        public T4 Item4 => Wrapper4.Value;
        public readonly LocatorLinkWrapper<T1> Wrapper1;
        public readonly LocatorLinkWrapper<T2> Wrapper2;
        public readonly LocatorLinkWrapper<T3> Wrapper3;
        public readonly LocatorLinkWrapper<T4> Wrapper4;
        public ComplexResolver(Action completeCallback, Container container) : base(completeCallback, container)
        {
            _completeMask = new IntCompleteMask(4);
            Wrapper1 = Container.GetLinkWrapper<T1>();
            Wrapper2 = Container.GetLinkWrapper<T2>();
            Wrapper3 = Container.GetLinkWrapper<T3>();
            Wrapper4 = Container.GetLinkWrapper<T4>();

            Wrapper1.OnValueSetEvent += CheckDependencies;
            Wrapper2.OnValueSetEvent += CheckDependencies;
            Wrapper3.OnValueSetEvent += CheckDependencies;
            Wrapper4.OnValueSetEvent += CheckDependencies;
        }
        public ComplexResolver(Action completeCallback, int containerID = 0) : this(completeCallback, ServiceLocatorObject.Instance.GetContainer(containerID)) { }

        public void CheckDependencies()
        {
            bool result = false;
            if (Wrapper1.CanBeResolved) result |= _completeMask.CompleteFlag(1);
            if (Wrapper2.CanBeResolved) result |= _completeMask.CompleteFlag(2);
            if (Wrapper3.CanBeResolved) result |= _completeMask.CompleteFlag(3);
            if (Wrapper4.CanBeResolved) result |= _completeMask.CompleteFlag(0);
            if (result) MakeComplete();
        }
        protected override void OnComplete()
        {
            Wrapper1.OnValueSetEvent -= CheckDependencies;
            Wrapper2.OnValueSetEvent -= CheckDependencies;
            Wrapper3.OnValueSetEvent -= CheckDependencies;
            Wrapper4.OnValueSetEvent -= CheckDependencies;
        }
    }
    public class ComplexResolver<T1, T2, T3, T4, T5> : ComplexResolver
    {
        public T1 Item1 => Wrapper1.Value;
        public T2 Item2 => Wrapper2.Value;
        public T3 Item3 => Wrapper3.Value;
        public T4 Item4 => Wrapper4.Value;
        public T5 Item5 => Wrapper5.Value;
        public readonly LocatorLinkWrapper<T1> Wrapper1;
        public readonly LocatorLinkWrapper<T2> Wrapper2;
        public readonly LocatorLinkWrapper<T3> Wrapper3;
        public readonly LocatorLinkWrapper<T4> Wrapper4;
        public readonly LocatorLinkWrapper<T5> Wrapper5;
        public ComplexResolver(Action completeCallback, Container container) : base(completeCallback, container)
        {
            _completeMask = new IntCompleteMask(5);
            var serviceLocator = ServiceLocatorObject.Instance.Container;
            Wrapper1 = serviceLocator.GetLinkWrapper<T1>();
            Wrapper2 = serviceLocator.GetLinkWrapper<T2>();
            Wrapper3 = serviceLocator.GetLinkWrapper<T3>();
            Wrapper4 = serviceLocator.GetLinkWrapper<T4>();
            Wrapper5 = serviceLocator.GetLinkWrapper<T5>();

            Wrapper1.OnValueSetEvent += CheckDependencies;
            Wrapper2.OnValueSetEvent += CheckDependencies;
            Wrapper3.OnValueSetEvent += CheckDependencies;
            Wrapper4.OnValueSetEvent += CheckDependencies;
            Wrapper5.OnValueSetEvent += CheckDependencies;
        }
        public ComplexResolver(Action completeCallback, int containerID = 0) : this(completeCallback, ServiceLocatorObject.Instance.GetContainer(containerID)) { }

        public void CheckDependencies()
        {
            bool result = false;
            if (Wrapper1.CanBeResolved) result |= _completeMask.CompleteFlag(1);
            if (Wrapper2.CanBeResolved) result |= _completeMask.CompleteFlag(2);
            if (Wrapper3.CanBeResolved) result |= _completeMask.CompleteFlag(3);
            if (Wrapper4.CanBeResolved) result |= _completeMask.CompleteFlag(4);
            if (Wrapper5.CanBeResolved) result |= _completeMask.CompleteFlag(0);
            if (result) MakeComplete();
        }
        protected override void OnComplete()
        {
            Wrapper1.OnValueSetEvent -= CheckDependencies;
            Wrapper2.OnValueSetEvent -= CheckDependencies;
            Wrapper3.OnValueSetEvent -= CheckDependencies;
            Wrapper4.OnValueSetEvent -= CheckDependencies;
            Wrapper5.OnValueSetEvent -= CheckDependencies;
        }
    }
}
