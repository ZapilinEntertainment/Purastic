using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface ILockedPinsContainer
    {
        public int CutPlaneA_id { get; }
        public int CutPlaneB_id { get; }
        public IReadOnlyCollection<FitElementPlaneAddress> GetLockedPinsA();
        public IReadOnlyCollection<FitElementPlaneAddress> GetLockedPinsB();
    }
    public class BlocksConnection // must be class - argument in events
    {
        public readonly int ID;
        public readonly PlacedBlock BlockA, BlockB;
        public readonly BlockFaceDirection DirectionA, DirectionB;
        public readonly ILockedPinsContainer LockedPins; 

        public BlocksConnection(int id, PlacedBlock blockA, PlacedBlock blockB, BlockFaceDirection dirA, BlockFaceDirection dirB, List<LockedPin> lockedPins)
        {
            ID = id;
            BlockA = blockA;
            BlockB = blockB;
            DirectionA = dirA;
            DirectionB = dirB;            

            if (lockedPins.Count == 2)
            {
                LockedPins = new TwoPinsContainer(lockedPins[0], lockedPins[1]);
            }
            else
            {
                List<FitElementPlaneAddress> listA = new(), listB = new();
                int keyA = lockedPins[0].CuttingPlaneID, keyB = -1;
                foreach (var pin in lockedPins)
                {
                    if (pin.CuttingPlaneID == keyA) listA.Add(pin.PlaneAddress);
                    else
                    {
                        keyB = pin.CuttingPlaneID;
                        listB.Add(pin.PlaneAddress);
                    }
                }
                LockedPins = new TwoListsContainer(keyA, listA, keyB, listB);
            }
        }

        
        private struct TwoPinsContainer : ILockedPinsContainer
        {
            private readonly LockedPin _pinA, _pinB;

            public TwoPinsContainer(LockedPin pinA, LockedPin pinB)
            {
                _pinA = pinA;
                _pinB = pinB;
            }
            public int CutPlaneA_id => _pinA.CuttingPlaneID;
            public int CutPlaneB_id => _pinB.CuttingPlaneID;
            public IReadOnlyCollection<FitElementPlaneAddress> GetLockedPinsA() => new FitElementPlaneAddress[1] { _pinA.PlaneAddress };
            public IReadOnlyCollection<FitElementPlaneAddress> GetLockedPinsB() => new FitElementPlaneAddress[1] { _pinB.PlaneAddress };
        }
        private struct TwoListsContainer : ILockedPinsContainer
        {
            private readonly int _idA, _idB;
            private readonly FitElementPlaneAddress[] _pinsA, _pinsB;

            public TwoListsContainer(int idA, List<FitElementPlaneAddress> pinsA, int idB, List<FitElementPlaneAddress> pinsB)
            {
                _idA = idA;
                _idB = idB;
                _pinsA = pinsA.ToArray();
                _pinsB = pinsB.ToArray();
            }

            public int CutPlaneA_id => _idA;
            public int CutPlaneB_id => _idB;
            public IReadOnlyCollection<FitElementPlaneAddress> GetLockedPinsA() => _pinsA;
            public IReadOnlyCollection<FitElementPlaneAddress> GetLockedPinsB() => _pinsB;
        }
    }
}
