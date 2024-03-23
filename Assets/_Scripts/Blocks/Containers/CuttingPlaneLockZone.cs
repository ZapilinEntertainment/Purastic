using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    // make a lock zone on a cutting plane, to prevent connections in it
    public class CuttingPlaneLockZone
    {
        public readonly ICuttingPlane _cuttingPlane;
        private readonly List<ConnectingPin> _lockedElements;
        public int PinsCount => _lockedElements.Count;
        public int CuttingPlaneID => _cuttingPlane.ID;
        public IReadOnlyList<ConnectingPin> LockedElements => _lockedElements;

        public CuttingPlaneLockZone(ICuttingPlane plane)
        {
            _cuttingPlane= plane;
            _lockedElements = new();
        }
        public CuttingPlaneLockZone(ICuttingPlane plane, IReadOnlyCollection<ConnectingPin> pinsList)
        {
            _cuttingPlane = plane;
            _lockedElements = new(pinsList);
        }

        public void AddLockedPin(ConnectingPin pin)
        {
            _lockedElements.Add(pin);
        }
        public void AddLockedPins(IReadOnlyCollection<ConnectingPin> pins)
        {
            _lockedElements.AddRange(pins);
        }
        public bool RemoveLockedPins(IReadOnlyCollection<ConnectingPin> pins)
        {
            foreach (var pin in pins)
            {
                _lockedElements.Remove(pin);
            }
            return _lockedElements.Count == 0;
        }
        public bool Contains(ConnectingPin address)
        {
            foreach (var element in _lockedElements)
            {
                if (element == address)
                {
                    //Debug.Log($"{element} x {address}");
                    return true;
                }
            }
            return false;
        }
        public bool Contains(int blockID, FitElementPlaneAddress address)
        {
            foreach (var element in _lockedElements)
            {
                if (element.BlockID == blockID && element.PlaneAddress == address) return true;
            }
            return false;
        }
        public bool CheckZoneForLockers(AngledRectangle rect)
        {           
            foreach (var element in _lockedElements)
            {
                if (rect.Contains(element.CutPlanePosition))
                {
                    return true;

                }
            }
            return false;
        }
        public bool IsSpaceLocked_CutPlanePosCheck(Vector2 cutPlanePosition)
        {
            foreach (var element in _lockedElements)
            {
                if (element.CutPlanePosition == cutPlanePosition)
                {
                    //Debug.Log($"{element} x {address}");
                    return true;
                }
            }
            return false;
        }
        public bool IsSpaceLocked_LocalPosCheck(Vector3 localPos)
        {
            foreach (var element in _lockedElements)
            {
                Vector3 elementLocalPos = _cuttingPlane.CutPlaneToLocalPos(element.CutPlanePosition);
                if (elementLocalPos == localPos)
                {
                    //Debug.Log($"{element} x {address}");
                    return true;
                }
            }
            return false;
        }    
    }
}
