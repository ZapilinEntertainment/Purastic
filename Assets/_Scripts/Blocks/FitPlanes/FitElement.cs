using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {

    public readonly struct FitElementFullAddress
    {
        public readonly Vector2 DetailSubPlanePosition; // IFitPlaneDataProvider
        public readonly Vector2Int DetailSubPlaneIndex;
        public readonly int DetailSubPlaneID; // id of subplane inside a cutPlane
        public readonly int CutPlaneID;
        public readonly BlockFaceDirection Face; // block face that element's plane belongs to. One face may contain a few planes
        public readonly int BlockID;
        public readonly int BaseplateID;
    }
    public readonly struct FitElement
	{
		public readonly FitType FitType;
		public readonly Vector2 PlanePosition;

		public FitElement(FitType type, Vector2 pos)
		{
			this.FitType = type;
			this.PlanePosition = pos;
		}
		public override int GetHashCode() => (FitType, PlanePosition.GetHashCode()).GetHashCode();

		public PinConnectionResult GetConnectionResult(FitElement other) => FitType.GetConnectResult(other.FitType);
    }
    // address of pin on the plane. A block face can contain multiple planes, so we need to store plane id.
    public readonly struct FitElementPlaneAddress
    {
        public readonly int PlaneContainerID; // not a cutplaneID, but an inner planes-container id
        public readonly Vector2Byte PinIndex;

        public FitElementPlaneAddress(int containerID, Vector2Byte index)
        {
            PlaneContainerID = containerID;
            PinIndex = index;
        }
        public FitElementPlaneAddress(Vector2Byte index)
        {
            PlaneContainerID = 0;
            PinIndex = index;
        }
        public FitElementPlaneAddress(int x, int y) : this(new Vector2Byte(x,y)) { }
    }
    public readonly struct FitElementPlanePosition
    {
        public readonly int SubPlaneId;
        public readonly Vector2 CutPlanePos;

        public FitElementPlanePosition(int SubPlaneID, Vector2 cutPlanePos)
        {
            SubPlaneId = SubPlaneID;
            CutPlanePos= cutPlanePos;
        }
    }
    public readonly struct ConnectingPin // for placing block
    {
        public readonly FitElement FitElement;
        public readonly FitElementPlaneAddress PlaneAddress;

        public FitType FitType => FitElement.FitType;
        public Vector2 CutPlanePosition => FitElement.PlanePosition;

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
            SubPlaneDataProviderID = pin.PlaneAddress.PlaneContainerID;
            CutPlanePosition = pin.FitElement.PlanePosition;
        }
        public FitElementPlanePosition ToPlanePosition() => new FitElementPlanePosition(SubPlaneDataProviderID, CutPlanePosition);
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
}
