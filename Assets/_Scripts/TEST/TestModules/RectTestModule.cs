using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {

	public sealed class RectTestModule : MonoBehaviour
	{
		[SerializeField] private Baseplate _baseplate;
        [SerializeField] private Vector2Int _fitPosition, _size;
        [SerializeField] private RotationStep _rotationStep;
        [SerializeField] private int _rotationValue;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || _baseplate == null) return;
            var rot2d = new Rotation2D(_rotationStep, _rotationValue);
            var rotation2d = rot2d.ToQuaternion();
            var rotation3d = rot2d.ToQuaternion(BlockFaceDirection.Up);
            Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, rotation3d, Vector3.one);
            var plane = _baseplate.GetPlatePlane();
            Vector3 zeroPos = plane.CutPlaneToLocalPos( plane.PlaneAddressToCutPlanePos(new(0, new Vector2Byte(_fitPosition))) + rot2d * (-0.5f * Vector2.one) );
            Vector3 size = new Vector3(_size.x, 1f, _size.y) * GameConstants.BLOCK_SIZE;
            Gizmos.DrawWireCube(zeroPos + size * 0.5f, size);
        }
#endif
    }
}
