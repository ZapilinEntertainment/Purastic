using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockModel : MonoBehaviour, IEquippable, IPlaceable, ICachableModel
	{
		private Block _block;
		public bool IsPlaceable => true;
        public bool IsVisible { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
        public GameObject ModelObject => gameObject;

        public Block GetBlockData() => _block;

        public void OnEquip(Transform handPoint)
        {
            transform.SetParent(handPoint, false);
			transform.localPosition = new Vector3(0f, _block.Size.z * 0.5f, _block.Size.y * 0.5f);
			transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
        }

        public void Setup(Block block, Material visualMaterial) {
			_block = block;
			SetDrawMaterial(visualMaterial);
		}
		public void SetDrawMaterial(Material material)
		{
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.sharedMaterial = material;
            }
        }

        public void SetPlacePosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public void Dispose() => Destroy(gameObject);
    }
}
