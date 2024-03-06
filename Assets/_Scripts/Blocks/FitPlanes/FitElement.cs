using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public readonly struct FitElement
	{
		public readonly FitType FitType;
		public readonly Vector2 CutPlanePosition;

		public FitElement(FitType type, Vector2 pos)
		{
			this.FitType = type;
			this.CutPlanePosition = pos;
		}
		public override int GetHashCode() => (FitType, CutPlanePosition.GetHashCode()).GetHashCode();

		public bool CanConnectWith(FitElement other) => FitType.GetConnectResult(other.FitType);
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
    }
    public readonly struct ConnectionPin
    {
        public readonly FitElement FitElement;
        public readonly FitElementPlaneAddress CutPlaneAddress;

        public FitType FitType => FitElement.FitType;
        public Vector2 CutPlanePosition => FitElement.CutPlanePosition;

        public ConnectionPin(FitElement element, FitElementPlaneAddress adress )
        {
            FitElement = element;
            CutPlaneAddress = adress;
        }
    }
    public readonly struct LockedPin
    {
        public readonly int CuttingPlaneID;
        public readonly FitElementPlaneAddress PlaneAddress;

        public LockedPin(int cutPlaneID, FitElementPlaneAddress address)
        {
            CuttingPlaneID = cutPlaneID;
            PlaneAddress = address;
        }
    }
    public readonly struct FitElementStructureAddress
    {
        public readonly int BlockID;
        public readonly BlockFaceDirection Direction;
        public readonly FitElementPlaneAddress PlanePinPosition;

        public readonly Vector3 WorldPosition;

        public FitElementStructureAddress(Vector3 modelPos)
        {
            WorldPosition = modelPos;
            BlockID = -1;
            Direction = default;
            PlanePinPosition = default;
        }
        public FitElementStructureAddress(int blockID, BlockFaceDirection direction, FitElementPlaneAddress planePinPosition, Vector3 position)
        {
            BlockID = blockID;
            Direction = direction;
            WorldPosition = position;
            PlanePinPosition = planePinPosition;
        }
    }
}
