using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public interface ILockedPinsContainer
    {
        public int CutPlaneA_id { get; }
        public int CutPlaneB_id { get; }
        public IReadOnlyCollection<ConnectingPin> GetLockedPinsA();
        public IReadOnlyCollection<ConnectingPin> GetLockedPinsB();
    }
    public class BlocksConnection // must be class - argument in events
    {
        public readonly int ID;
        public readonly PlacedBlock BlockA, BlockB;
        public readonly BlockFaceDirection DirectionA, DirectionB;
        public readonly ILockedPinsContainer LockedPins; 

        public BlocksConnection(int id, PlacedBlock blockA, PlacedBlock blockB, ICuttingPlane newBlockCutPlane, ConnectedAndLockedPinsContainer pinsContainer)
        {
            ID = id;
            BlockA = blockA;
            BlockB = blockB;
            var planeA = pinsContainer.BasementCutPlane;
            DirectionA = planeA.Face;
            DirectionB = newBlockCutPlane.Face;            

            if (pinsContainer.PairsCount == 1)
            {
                LockedPins = new TwoPinsContainer(planeA.ID, pinsContainer.BasementConnectedPins[0], newBlockCutPlane.ID, pinsContainer.NewBlockConnectedPins[0]);
            }
            else
            {
                LockedPins = new TwoListsContainer(planeA.ID, pinsContainer.BasementConnectedPins, newBlockCutPlane.ID, pinsContainer.NewBlockConnectedPins);
            }
        }

        
        private struct TwoPinsContainer : ILockedPinsContainer
        {
            private readonly int _idA, _idB;
            private readonly ConnectingPin _pinA, _pinB;

            public TwoPinsContainer(int idA, ConnectingPin pinA, int idB, ConnectingPin pinB)
            {
                _idA = idA;
                _idB = idB;
                _pinA = pinA;
                _pinB = pinB;
            }
            public int CutPlaneA_id => _idA;
            public int CutPlaneB_id => _idB;
            public IReadOnlyCollection<ConnectingPin> GetLockedPinsA() => new ConnectingPin[1] { _pinA };
            public IReadOnlyCollection<ConnectingPin> GetLockedPinsB() => new ConnectingPin[1] { _pinB };
        }
        private struct TwoListsContainer : ILockedPinsContainer
        {
            private readonly int _idA, _idB;
            private readonly ConnectingPin[] _pinsA, _pinsB;

            public TwoListsContainer(int idA, List<ConnectingPin> pinsA, int idB, List<ConnectingPin> pinsB)
            {
                _idA = idA;
                _idB = idB;
                _pinsA = pinsA.ToArray();
                _pinsB = pinsB.ToArray();
            }

            public int CutPlaneA_id => _idA;
            public int CutPlaneB_id => _idB;
            public IReadOnlyCollection<ConnectingPin> GetLockedPinsA() => _pinsA;
            public IReadOnlyCollection<ConnectingPin> GetLockedPinsB() => _pinsB;
        }
    }
}
