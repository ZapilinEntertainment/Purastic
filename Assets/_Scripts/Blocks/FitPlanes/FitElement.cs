using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public readonly struct FitElementFullAddress
    {
        public readonly Vector2 DetailSubPlanePosition; // position inside plane
        public readonly Vector2 CutPlanePosition; // position on structure cut plane// FitElementCutPlanePosition

        public readonly Vector2Int DetailSubPlaneIndex; // FitElementPlaneAddress

        public readonly byte DetailPlaneID; // index of block plane. Every block have at least one plane
        public readonly int CutPlaneID;
        public readonly BlockFaceDirection Face; // block face that element's plane belongs to. One face may contain a few planes
        public readonly int BlockID;
        public readonly int BaseplateID;
    }
    public readonly struct FitElement : IEquatable<FitElement> 
	{
		public readonly FitType FitType;
		public readonly Vector2 Position;

		public FitElement(FitType type, Vector2 position)
		{
			this.FitType = type;
			this.Position = position;
		}
        #region equality
        public override bool Equals(object obj) => obj is FitElement other && this.Equals(other);
        public bool Equals(FitElement p) => FitType == p.FitType && Position == p.Position;
        public override int GetHashCode() => (FitType, Position).GetHashCode();
        public static bool operator ==(FitElement lhs, FitElement rhs) => lhs.Equals(rhs);
        public static bool operator !=(FitElement lhs, FitElement rhs) => !(lhs == rhs);
        #endregion

		public PinConnectionResult GetConnectionResult(FitElement other) => FitType.GetConnectResult(other.FitType);
    }
    
    // address of pin on the plane. A block face can contain multiple planes, so we need to store plane id.
    public readonly struct FitElementPlaneAddress : IEquatable<FitElementPlaneAddress>
    {
        public readonly byte SubPlaneId; // not a cutplaneID, but an inner planes-container id
        public readonly Vector2Byte PinIndex;

        public FitElementPlaneAddress(byte planeId, Vector2Byte index)
        {
            SubPlaneId = planeId;
            PinIndex = index;
        }
        public FitElementPlaneAddress(Vector2Byte index)
        {
            SubPlaneId = 0;
            PinIndex = index;
        }
        public FitElementPlaneAddress(int x, int y) : this(new Vector2Byte(x,y)) { }
        #region equality
        public override bool Equals(object obj) => obj is FitElementPlaneAddress other && this.Equals(other);
        public bool Equals(FitElementPlaneAddress p) => SubPlaneId == p.SubPlaneId && PinIndex == p.PinIndex;
        public override int GetHashCode() => (SubPlaneId, PinIndex).GetHashCode();
        public static bool operator ==(FitElementPlaneAddress lhs, FitElementPlaneAddress rhs) => lhs.Equals(rhs);
        public static bool operator !=(FitElementPlaneAddress lhs, FitElementPlaneAddress rhs) => !(lhs == rhs);
        #endregion
        public override string ToString() => $"plane {SubPlaneId}: {PinIndex}";
    }
    public readonly struct FitElementFacePosition
    {
        public readonly byte SubPlaneId;
        public readonly Vector2 PlanePos;

        public FitElementFacePosition(byte SubPlaneID, Vector2 planePos)
        {
            SubPlaneId = SubPlaneID;
            PlanePos= planePos;
        }
    }
    public readonly struct FitElementPlanePosition : IEquatable<FitElementPlanePosition> 
    {
        public readonly Vector2 PlanePosition;
        public readonly Vector2Byte Index;

        public FitElementPlanePosition(Vector2Byte index, Vector2 planePosition)
        {
            PlanePosition = planePosition;
            Index = index;
        }
        public FitElementPlanePosition(byte x, byte y, Vector2 planePosition) : this(new Vector2Byte(x, y), planePosition) { }

        #region equality
        public override bool Equals(object obj) => obj is FitElementPlanePosition other && this.Equals(other);
        public bool Equals(FitElementPlanePosition p) => Index == p.Index && PlanePosition == p.PlanePosition;
        public override int GetHashCode() => (Index, PlanePosition).GetHashCode();
        public static bool operator ==(FitElementPlanePosition lhs, FitElementPlanePosition rhs) => lhs.Equals(rhs);
        public static bool operator !=(FitElementPlanePosition lhs, FitElementPlanePosition rhs) => !(lhs == rhs);
        #endregion
    }

    public readonly struct ConnectingPin : IEquatable<ConnectingPin>
        // for placing and locking block
    {
        public readonly int BlockID;
        public readonly FitElement FitElement;
        public readonly FitElementPlaneAddress PlaneAddress;

        public FitType FitType => FitElement.FitType;
        public Vector2 CutPlanePosition => FitElement.Position;

        public ConnectingPin(int blockID,FitElement element, FitElementPlaneAddress adress )
        {
            BlockID = blockID;
            FitElement = element;
            PlaneAddress = adress;
        }
        public ConnectingPin ChangeID(int id) => new ConnectingPin(id, FitElement, PlaneAddress);

        #region equality
        public override bool Equals(object obj) => obj is ConnectingPin other && this.Equals(other);
        public bool Equals(ConnectingPin p) => BlockID == p.BlockID && PlaneAddress == p.PlaneAddress && FitElement == p.FitElement;
        public override int GetHashCode() => (BlockID, PlaneAddress).GetHashCode();
        public static bool operator ==(ConnectingPin lhs, ConnectingPin rhs) => lhs.Equals(rhs);
        public static bool operator !=(ConnectingPin lhs, ConnectingPin rhs) => !(lhs == rhs);
        #endregion
    }
    public readonly struct FitElementCutPlanePosition
    {
        public readonly int CutPlaneID;
        public readonly byte SubPlaneDataProviderID;
        public readonly Vector2 CutPlanePosition;
        
        public FitElementCutPlanePosition(int cutPlaneId, byte subPlaneId, Vector2 cutPlanePos)
        {
            CutPlaneID = cutPlaneId;
            SubPlaneDataProviderID = subPlaneId;
            CutPlanePosition = cutPlanePos;
        }
        public FitElementCutPlanePosition(int cutPlaneID, ConnectingPin pin)
        {
            CutPlaneID = cutPlaneID;
            SubPlaneDataProviderID = pin.PlaneAddress.SubPlaneId;
            CutPlanePosition = pin.FitElement.Position;
        }
    }
    public readonly struct FitElementStructureAddress
    {
        public readonly int BlockID;
        public readonly int CutPlaneID;
        public readonly BlockFaceDirection ContactFace;
        public readonly FitElementPlaneAddress PlaneAddress;

        public FitElementStructureAddress(int blockID,int cutPlaneID, BlockFaceDirection direction, FitElementPlaneAddress planePinPosition)
        {
            BlockID = blockID;
            CutPlaneID = cutPlaneID;
            ContactFace = direction;
            PlaneAddress = planePinPosition;
        }
        public FitElementStructureAddress(int blockID, int cutPlaneID, FaceDirection direction, FitElementPlaneAddress planePinPosition)
        {
            BlockID = blockID;
            CutPlaneID = cutPlaneID;
            ContactFace = new BlockFaceDirection(direction);
            PlaneAddress = planePinPosition;
        }
    }
    public readonly struct FoundedFitElementPosition
    {
        public readonly bool PositionIsObstructed;
        public readonly int BlockHostID;
        public readonly Vector3 HitPlaneNormal;
        public readonly FitElementStructureAddress StructureAddress;
        public readonly VirtualPoint WorldPoint;

        public FoundedFitElementPosition(int blockHostID, FitElementStructureAddress structureAddress, VirtualPoint worldPoint, Vector3 normal, bool positionIsObstructed)
        {
            BlockHostID = blockHostID;
            StructureAddress = structureAddress;
            WorldPoint = worldPoint;
            HitPlaneNormal = normal;
            PositionIsObstructed = positionIsObstructed;
        }
    }
}
