using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum BlockRotationType: byte
	{
		Horizontal90DegStep
	}
	public struct PlacedBlockRotation
	{
		public readonly byte HorizontalRotationStep;
		public readonly BlockRotationType RotationType;

		public Vector3 Forward => Quaternion * Vector3.forward;
		public Quaternion Quaternion => Quaternion.AngleAxis(90f * HorizontalRotationStep, Vector3.up);

    }
}
