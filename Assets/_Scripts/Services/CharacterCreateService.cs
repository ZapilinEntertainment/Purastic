using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using System;

namespace ZE.Purastic {
	public sealed class CharacterCreateService : IContainable
	{
		private PlayableCharacter.Factory _factory;
		public int ContainerID { get; set; }

		public CharacterCreateService()
		{
			_factory = ServiceLocatorObject.GetFactory<PlayableCharacter.Factory>(ContainerID);
		}
		public async Awaitable<PlayableCharacter> CreateCharacter()
		{
			if (_factory == null) await WaitForFactoryInitialize();
			return _factory.Create();
		}

		private async Awaitable WaitForFactoryInitialize()
		{
			do
			{
				await Awaitable.FixedUpdateAsync();
			}
			while (_factory == null);
		}
	}
}
