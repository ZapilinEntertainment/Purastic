using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/CameraSettings")]
    public sealed class CameraSettings : ScriptableObject
	{
        [field: SerializeField] public float FovChangeSpeed { get; private set; } = 0.03f;
        [field: SerializeField] public float OffsetChangeSpeed { get; private set; } = 0.02f;

        [field: SerializeField] public float MaxFov { get; private set; } = 90f;
        [field: SerializeField] public float MaxCameraSpeed { get; private set; } = 100f;
        [field: SerializeField] public float MaxOffsetY { get; private set; } = 10f;
    }
}
