using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public enum FitElementSpace : byte { Plane, Face, CutPlane}
    public readonly struct FitElementFullAddress
    {
        public readonly Vector2 DetailSubPlanePosition; // position inside plane
        public readonly Vector2 FacePlanePosition; // position inside face (face may contain several planes) //  FitElementFacePosition
        public readonly Vector2 CutPlanePosition; // position on structure cut plane// FitElementCutPlanePosition

        public readonly Vector2Int DetailSubPlaneIndex; // FitElementPlaneAddress

        public readonly byte DetailPlaneID; // index of block plane. Every block have at least one plane
        public readonly int CutPlaneID;
        public readonly BlockFaceDirection Face; // block face that element's plane belongs to. One face may contain a few planes
        public readonly int BlockID;
        public readonly int BaseplateID;
    }
    public readonly struct FitElement
	{
		public readonly FitType FitType;
        public readonly FitElementSpace Space;
		public readonly Vector2 Position;

		public FitElement(FitType type, FitElementSpace space, Vector2 position)
		{
			this.FitType = type;
			this.Position = position;
            this.Space= space;
		}
		public override int GetHashCode() => (FitType, Position.GetHashCode()).GetHashCode();

		public PinConnectionResult GetConnectionResult(FitElement other) => FitType.GetConnectResult(other.FitType);
    }


    public readonly struct FitElementPlanePosition
    {
        public readonly Vector2 PlanePosition;
        public readonly Vector2Byte Index;

        public FitElementPlanePosition(Vector2Byte index, Vector2 planePosition)
        {
            PlanePosition = planePosition;
            Index = index;
        }
        public FitElementPlanePosition(byte x, byte y, Vector2 planePosition) : this(new Vector2Byte(x, y), planePosition) { }
    }
    // address of pin on the plane. A block face can contain multiple planes, so we need to store plane id.
    public readonly struct FitElementPlaneAddress
    {
        public readonly int SubPlaneId; // not a cutplaneID, but an inner planes-container id
        public readonly Vector2Byte PinIndex;

        public FitElementPlaneAddress(int containerID, Vector2Byte index)
        {
            SubPlaneId = containerID;
            PinIndex = index;
        }
        public FitElementPlaneAddress(Vector2Byte index)
        {
            SubPlaneId = 0;
            PinIndex = index;
        }
        public FitElementPlaneAddress(int x, int y) : this(new Vector2Byte(x,y)) { }

        public override string ToString() => $"plane {SubPlaneId}: {PinIndex}";
    }
    public readonly struct FitElementFacePosition
    {
        public readonly int SubPlaneId;
        public readonly Vector2 PlanePos;

        public FitElementFacePosition(int SubPlaneID, Vector2 planePos)
        {
            SubPlaneId = SubPlaneID;
            PlanePos= planePos;
        }
    }    
    public readonly struct ConnectingPin // for placing block
    {
        public readonly FitElement FitElement;
        public readonly FitElementPlaneAddress PlaneAddress;

        public FitType FitType => FitElement.FitType;
        public Vector2 CutPlanePosition => FitElement.Position;

        public ConnectingPin(FitElement element, FitElementPlaneAddress adress )
        {
            FitElement = element;
            PlaneAddress = adress;
        }
    }
    public readonly struct FitElementCutPlanePosition
    {
        public readonly int CutPlaneID;
        public readonly int SubPlaneDataProviderID;
        public readonly Vector2 CutPlanePosition;
        
        public FitElementCutPlanePosition(int cutPlaneId, int subPlaneId, Vector2 cutPlanePos)
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
        public readonly FitElementStructureAddress StructureAddress;
        public readonly VirtualPoint WorldPoint;

        public FoundedFitElementPosition(FitElementStructureAddress structureAddress, VirtualPoint worldPoint)
        {
            StructureAddress = structureAddress;
            WorldPoint = worldPoint;
        }
    }
}
