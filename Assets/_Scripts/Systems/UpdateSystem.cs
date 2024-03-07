using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	[DefaultExecutionOrder(10)]
	public sealed class UpdateSystem : MonoBehaviour
	{
		public Action OnUpdateEvent;
        public Action OnFixedUpdateEvent;
        public Action<float> OnUpdateTimeEvent;
        public Action<float> OnFixedUpdateTimeEvent;

        private void Update()
        {
            OnUpdateEvent?.Invoke();
            if (OnUpdateTimeEvent != null) {
                float t = Time.deltaTime;
                OnUpdateTimeEvent.Invoke(t);
            }
        }
        private void FixedUpdate()
        {
            OnFixedUpdateEvent?.Invoke();
            if (OnFixedUpdateTimeEvent != null)
            {
                float t = Time.fixedDeltaTime;
                OnFixedUpdateTimeEvent.Invoke(t);
            }
        }
    }
}
