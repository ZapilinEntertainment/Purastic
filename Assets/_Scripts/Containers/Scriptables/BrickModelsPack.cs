using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    [CreateAssetMenu(menuName = "ScriptableObjects/BrickModelsPack")]
    public sealed class BrickModelsPack : ScriptableObject
	{
        [field: SerializeField] public GameObject CubePrefab { get; private set; }
        [field: SerializeField] public GameObject KnobPrefab { get; private set; }
    }
}
