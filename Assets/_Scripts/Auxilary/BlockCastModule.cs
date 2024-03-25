using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockCastModule
	{
        private int _castMask;
        private Transform _marker;
        private ComplexResolver<InputController, CameraController, ColliderListSystem> _resolver;
        private InputController _inputController;
        private CameraController _cameraController;
        private ColliderListSystem _colliders;
        public bool IsReady { get; private set; } = false;

        public BlockCastModule()
        {
            _castMask = LayerConstants.GetCustomLayermask(CustomLayermask.BlockPlaceCast);
            _resolver = new(OnDependendicesResolved);
            _resolver.CheckDependencies();
        }
        private void OnDependendicesResolved()
        {
            _inputController = _resolver.Item1;
            _cameraController = _resolver.Item2;
            _colliders = _resolver.Item3;
            
            IsReady = true;
        }

        public bool Cast(out FoundedFitElementPosition position, out RaycastHit hit)
		{
            if (Cast(out IBlocksHost host, out hit))
            {
                return host.TryGetFitElementPosition(hit, out position);
            }
            else position = default;
            return false;
        }
        public bool Cast(out IBlocksHost host, out RaycastHit hit)
        {
            if (_cameraController.TryRaycast(_inputController.CursorPosition, out hit, _castMask))
            {
                if (_colliders.TryDefineBlockhost(hit.colliderInstanceID, out host))
                {
                    return true;

                }
            }
            host = null;
            hit= default;
            return false;
        }
    }
}
