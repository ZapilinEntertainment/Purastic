using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public readonly struct FitElement
	{
		public readonly FitType FitType;
		public readonly Vector3 Position;

		public FitElement(FitType type, Vector3 pos)
		{
			this.FitType = type;
			this.Position = pos;
		}
		public override int GetHashCode() => (FitType, Position.GetHashCode()).GetHashCode();

		public bool CanConnectWith(FitElement other) => FitType.CanConnect(other.FitType);
    }
}
