using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class CharacterModel : MonoBehaviour, IColliderOwner
	{
		[SerializeField] private Collider _collider;
		[field: SerializeField] public Transform ZeroPoint { get; private set; }
        [field: SerializeField] public Transform ViewPoint { get; private set; }
        [field: SerializeField] public Transform HandPoint { get; private set; }

        public bool HaveMultipleColliders => false;
		public int GetColliderID() => _collider.GetInstanceID();
		public IReadOnlyCollection<int> GetColliderIDs() => new int[1] { GetColliderID() };

        public void SetView(Vector3 dir) =>  transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

		private class RotationHandler
		{			
			private float _rotationProgress = 0f;
			private Quaternion _rotationTarget, _startRotation;
			private readonly Transform _transform;
			private readonly RotationConfiguration _config;
			public bool IsRotating { get; private set; } = false;

			public RotationHandler(RotationConfiguration rotateConfig, Transform transform)
			{
				_config = rotateConfig;
				_transform = transform;
			}

			public void Update(float deltaTime)
			{
				_rotationProgress = Mathf.MoveTowards(_rotationProgress, 1f, deltaTime / _config.RotationTime);
				_transform.rotation = Quaternion.Lerp(_startRotation, _rotationTarget, _config.RotationCurve.Evaluate(_rotationProgress));
				IsRotating = _rotationProgress != 1f;
			}
			public void SetRotationTarget(Quaternion rotation)
			{
				_startRotation = _transform.rotation;
				_rotationTarget = rotation;
				IsRotating = true;
				_rotationProgress = 0f;
			}
		}
		[System.Serializable]
		private class RotationConfiguration
		{
			public float RotationTime = 0.5f;
			public AnimationCurve RotationCurve;
		}
    }
}
