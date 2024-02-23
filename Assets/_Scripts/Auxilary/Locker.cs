using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZE.Purastic {
    public class Locker
    {
        public bool IsLocked { get; private set; }
        private HashSet<int> _activeLocks = new HashSet<int>();
        private int _nextLockId = 1;
        public System.Action OnLockStartEvent, OnLockEndEvent;

        public int CreateLock()
        {
            int id = _nextLockId++;
            _activeLocks.Add(id);
            if (!IsLocked)
            {
                IsLocked = true;
                OnLockEndEvent?.Invoke();
            }
            return id;
        }

        public void DeleteLock(int id)
        {
            _activeLocks.Remove(id);
            if (IsLocked)
            {
                IsLocked = _activeLocks.Count > 0;
                if (!IsLocked) OnLockEndEvent?.Invoke();
            }
        }
        public void ClearAllLocks()
        {
            _activeLocks.Clear();
            if (IsLocked)
            {
                IsLocked = false;
                OnLockEndEvent?.Invoke();
            }
        }
    }
}
