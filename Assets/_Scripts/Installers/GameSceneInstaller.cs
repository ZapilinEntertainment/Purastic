using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {
	public sealed class GameSceneInstaller : InstallerBase
	{
        [SerializeField] private GameResourcesPack _resourcesPack;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private NetworkManager _networkManager;
        protected override void OnInstall()
        {
            RegisterInstance(new SignalBus(_container.ID));            
            RegisterInstance(_cameraController);
            RegisterInstance(_networkManager);

            RegisterInstance(_resourcesPack);
            RegisterInstance(_resourcesPack.MaterialsPack);
            RegisterInstance(_resourcesPack.BrickModelsPack);

            RegisterInstruction<CharacterCreateService>();
            RegisterInstruction<BlockCreateService>();
            RegisterInstruction<BlockModelPoolService>();

            RegisterInstruction<ColliderListSystem>();
            RegisterMonoComponentInstruction<BlockPlaceSystem>(true);

            RegisterInstruction<MaterialsDepot>();
        }
    }
}
