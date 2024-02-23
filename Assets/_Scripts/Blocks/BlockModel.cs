using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockModel : MonoBehaviour, IEquippable
	{
		private Block _block;

        public void OnEquip(Transform handPoint)
        {
            transform.SetParent(handPoint, false);
			transform.localPosition = new Vector3(0f, _block.Size.z * 0.5f, _block.Size.y * 0.5f);
			transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
        }

        public void Setup(Block block) {
			_block = block;
			var renderers = GetComponentsInChildren<Renderer>(true);
			foreach (var renderer in renderers)
			{
				renderer.sharedMaterial = block.Material.VisibleMaterial;
			}
		}
	}
}
