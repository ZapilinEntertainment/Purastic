using System.Collections;
using System.Collections.Generic;
using System;

namespace ZE.ServiceLocator
{
    public interface IResolvable
    {
        public bool CanBeResolved { get; }
        public object ValueLink { get; }
        public Action OnValueSetEvent { get; set; }
    }
    public interface ILinkWrapper
    {
        public void SetLink(object T);
    }
    public class LocatorLinkWrapper<T> : IResolvable, ILinkWrapper
    {
        public bool CanBeResolved { get; private set; }
        public T Value { get; private set; }
        public object ValueLink => Value;
        public Action OnValueSetEvent { get; set; }

        public LocatorLinkWrapper()
        {
            Value = default;
            CanBeResolved = false;
        }
        public LocatorLinkWrapper(T link)
        {
            SetLink(link);
        }
        public void SetLink(T link)
        {
            Value = link;
            CanBeResolved = Value != null;
            if (CanBeResolved) {
                OnValueSetEvent?.Invoke();
                }
        }
        public void SetLink(object link)
        {
            if (link.GetType() == typeof(T)) SetLink((T)link);
        }
    }
}
