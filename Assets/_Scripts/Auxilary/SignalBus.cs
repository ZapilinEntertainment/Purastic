using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic{
	public sealed class SignalBus
	{
		private readonly int _containerID;
        private readonly Type _baseManifestationType = typeof(SignalManifestation<>);
        private readonly Dictionary<Type, ISignalManifestation> _activeSignals = new();
		public SignalBus(int containerID) {
			_containerID = containerID;
		}

		public void SubscribeToSignal<T>(Action onSignalFire) where T : ISignal => SubscribeToSignal(typeof(T), onSignalFire, () => new SignalManifestation<T>());
        public void SubscribeToSignal<T>(Action<T> onSignalFire) where T : ISignal
		{
			var key = typeof(T);
            if (_activeSignals.TryGetValue(key, out var manifestation))
			{
				(manifestation as SignalManifestation<T>).OnSignalFiredWithArgumentEvent+= onSignalFire;
			}
			else
			{
				var manifest = new SignalManifestation<T>();
				manifest.OnSignalFiredWithArgumentEvent+= onSignalFire;
				_activeSignals.Add(key, manifest);
			}
		}
		public void SubscribeToSignal(ISignal signal, Action onSignalFired) { 
			var type = signal.GetType();
			SubscribeToSignal(type, onSignalFired, () => (ISignalManifestation)Activator.CreateInstance(_baseManifestationType.MakeGenericType(type)));
		}
		private void SubscribeToSignal(Type type, Action onSignalFired, Func<ISignalManifestation> createFunc)
		{
            ISignalManifestation manifest;
            if (!_activeSignals.TryGetValue(type, out manifest))
            {
                manifest = createFunc();
                _activeSignals.Add(type, manifest);
            }
            manifest.OnSignalFiredEvent += onSignalFired;
        }
        public void FireSignal<T>() where T: ISignal
		{
			if (_activeSignals.TryGetValue(typeof(T), out var manifestation))
			{
				manifestation.Fire();
			}
		}
        public void FireSignal<T>(T signal) where T : ISignal
        {
			var key = typeof(T);
            if (_activeSignals.TryGetValue(key, out var manifestation))
            {
                manifestation.Fire(signal);
            }
        }

        private interface ISignalManifestation {
			public Action OnSignalFiredEvent { get; set; }

			public void Fire();
			public void Fire(ISignal argument);
		}
		private class SignalManifestation<T> : ISignalManifestation where T:ISignal
		{
			public Action OnSignalFiredEvent { get; set; }
            public Action<T> OnSignalFiredWithArgumentEvent;

			public void Fire()
			{
				OnSignalFiredEvent();
			}
			public void Fire(ISignal argument)
			{
				var signal = (T)argument;
				OnSignalFiredEvent?.Invoke();
				OnSignalFiredWithArgumentEvent?.Invoke(signal);
			}
		}
	}

	public interface ISignal { }
	public class CameraViewPointChangedSignal : ISignal {
		public readonly ViewPointInfo ViewPointInfo;
		public CameraViewPointChangedSignal(ViewPointInfo viewPointInfo) { ViewPointInfo= viewPointInfo; }
	}
	public class ActivateBlockPlaceSystemSignal : ISignal {
		public readonly IBlockPlacer BlockPlacer;
		public ActivateBlockPlaceSystemSignal(IBlockPlacer blockPlacer)
		{
			BlockPlacer= blockPlacer;
		}
	}
	public class DeactivateBlockPlaceSystemSignal : ISignal { }
}
