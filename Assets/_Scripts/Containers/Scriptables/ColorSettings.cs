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
        [field: SerializeField] public Color PlaceBlockColor_Obstructed { get; private set; } = Color.red;

        public Color GetPlaceColor(BlockPositionStatus status)
        {
            switch (status) { 
                case BlockPositionStatus.CanBePlaced: return PlaceBlockColor_Available;
                case BlockPositionStatus.Obstructed: return PlaceBlockColor_Obstructed;
                default: return PlaceBlockColor_Unavailable;
            }
        }
    }
}
