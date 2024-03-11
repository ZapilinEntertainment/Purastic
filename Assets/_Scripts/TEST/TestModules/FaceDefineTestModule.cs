using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class FaceDefineTestModule : MonoBehaviour
	{
		private bool _isReady = false;
		private BlockCastModule _castModule;

		private async void Start()
		{
			//var rect = new AngledRectangle(new Rect(-8f, 8f, 16f, 16f), Rotation2D.NoRotation);
			

			_castModule = new BlockCastModule();
			while (!_castModule.IsReady) await Awaitable.FixedUpdateAsync();
			_isReady = true;
		}
        private void FixedUpdate()
        {
            if (_isReady && _castModule.Cast(out IBlocksHost host, out var hit))
			{
				Debug.Log(new BlockFaceDirection(hit.normal));
			} 
        }
    }
}
