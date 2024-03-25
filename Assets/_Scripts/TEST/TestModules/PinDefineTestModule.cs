using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PinDefineTestModule : MonoBehaviour
	{
        private bool _pinFound = false;
        private FoundedFitElementPosition _fitPosition;
        private BlockCastModule f_castModule;

        virtual protected bool IsReady => CastModule.IsReady;
        protected bool PinFound => _pinFound;
        protected FoundedFitElementPosition PositionInfo => _fitPosition;
        protected BlockCastModule CastModule
        {
            get
            {
                if (f_castModule == null) f_castModule = new();
                return f_castModule;
            }
        }

        protected void FixedUpdate()
        {
            if (IsReady)
            {                
                var pinFound = CastModule.Cast(out _fitPosition, out var hit);
                if (pinFound) OnPinFound(hit);
                else OnPinLost();
                OnFixedUpdate(hit);
            }
        }
        virtual protected void OnFixedUpdate(RaycastHit rh) { }
        virtual protected void OnPinFound(RaycastHit rh) {
            _pinFound = true;
        }
        virtual protected void OnPinLost() {
            _pinFound = false;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            if (Application.isPlaying && enabled)
            {
               if (PinFound) OnDrawPinGizmos();
                OnDrawAnyGizmos();
            }
        }
#endif

        virtual protected void OnDrawPinGizmos()
        {
#if UNITY_EDITOR
            Gizmos.DrawSphere(PositionInfo.WorldPoint.Position, 0.5f);
#endif
        }
        virtual protected void OnDrawAnyGizmos() { }

    }
}
