using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class Baseplate : MonoBehaviour
	{
		[field:SerializeField] public byte Width = 16, Length = 16;
        [SerializeField] private BlockMaterial _material = new BlockMaterial();
		private ConstructionVisualizer _visualizer;

        public Block ToBlock() => new Block(
            new BaseplateGrid(Width, Length),
            new Vector3(Width * GameConstants.BLOCK_SIZE, GameConstants.PLATE_THICK, Length * GameConstants.BLOCK_SIZE),
            _material
            );

        private void Start()
        {
            _visualizer = new ConstructionVisualizer(this);
        }
    }
}
