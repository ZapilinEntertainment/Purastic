using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/ColorSettings")]
    public sealed class ColorSettings : ScriptableObject
	{
		[field: SerializeField] public Color PlaceBlockColor_Unavailable { get; private set; } = Color.white;
        [field: SerializeField] public Color PlaceBlockColor_Available { get; private set; } = Color.yellow;
    }
}
