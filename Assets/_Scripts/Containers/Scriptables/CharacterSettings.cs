using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/CharacterSettings")]
	public class CharacterSettings : ScriptableObject
	{
        [field:SerializeField] public JumpConfig JumpConfig { get; private set; }
        [field: SerializeField] public GravityConfig GravityConfig { get; private set; }
        [field: SerializeField] public MoveConfig MoveConfig { get; private set; }
        [field: SerializeField] public PointViewSettings ViewSettings { get; private set; }
    }
}
