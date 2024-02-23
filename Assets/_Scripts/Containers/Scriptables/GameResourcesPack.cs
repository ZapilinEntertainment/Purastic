using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/GameResourcesPack")]
    public sealed class GameResourcesPack : ScriptableObject
	{
		[field: SerializeField] public PlayableCharacter DefaultCharacter { get; private set; }
		[field: SerializeField] public GameObject KnobPrefab { get; private set; }
	}
}
