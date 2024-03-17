using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class ConstructionVisualizerModule : MonoBehaviour
	{
		private bool _needRedraw = false;
		private MultiFlagsCondition _dependencyFlags;
		private ComplexResolver<IBlocksHost, PlacedBlocksListHandler, CuttingPlanesManager> _localResolver;
		private BlockCreateService BlockCreateService => _outerResolver.Item1;
		private BlockModelPoolService CacheService => _outerResolver.Item3;
		private IBlocksHost BlocksHost => _localResolver.Item1;
		private PlacedBlocksListHandler BlocksList=> _localResolver.Item2;
        private ComplexResolver<BlockCreateService, GameResourcesPack, BlockModelPoolService> _outerResolver;
		private List<BlockModel> _models = new();

		public void Setup(Container container) {
			_dependencyFlags = new MultiFlagsCondition(3, OnAllDependenciesResolved);

			_localResolver = new(OnLocalContainerResolved, container);
            _outerResolver = new(() => _dependencyFlags.CompleteFlag(0));
			_outerResolver.CheckDependencies();
			_localResolver.CheckDependencies();
        }

		private void OnLocalContainerResolved()
		{
			_dependencyFlags.CompleteFlag(1);
            if (BlocksHost.IsInitialized)
			{
				_dependencyFlags.CompleteFlag(2);		
			}
			else
			{
				BlocksHost.InitStatusModule.OnInitializedEvent += () => _dependencyFlags.CompleteFlag(2);
			}
			_localResolver.Item3.OnLockedZonesChangedEvent += OnLockPointsChanged;
        }
		private void OnAllDependenciesResolved()
		{
			var blocks = BlocksList.GetPlacedBlocks();
			foreach (var block in blocks)
			{
				OnBlockPlaced(block);
			}
            BlocksHost.OnBlockPlacedEvent += OnBlockPlaced;
        }


        private void Update()
        {
			if (_needRedraw) FullRedrawAsync();
        }

        private async void OnBlockPlaced(PlacedBlock block)
		{
            var model = await BlockCreateService.CreateBlockModel(block.Properties);
			var modelTransform = model.transform;
            modelTransform.SetParent(BlocksHost.ModelsHost, false);
			modelTransform.SetLocalPositionAndRotation(block.LocalPosition, block.Rotation);
			model.AssignHost(BlocksHost, block);
            _models.Add(model);
        }
		private void OnLockPointsChanged(CuttingPlanePosition coordinate, CuttingPlaneLockZone lockZone)
		{
			foreach (var model in _models)
			{
				model.OnPinsLocked(coordinate, lockZone);
			}
		}
		public async void FullRedrawAsync()
		{
			int count = _models.Count;
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					CacheService.CacheModel(_models[i]);
				}
				_models.Clear();
			}			
			var blockData = BlocksHost.GetBlocks();
			Transform host = BlocksHost.ModelsHost;
			foreach (var data in blockData)
			{
                var block = await BlockCreateService.CreateBlockModel(data);
                block.transform.SetParent(host, false);
				_models.Add(block);
            }
			
		}
	}
}
