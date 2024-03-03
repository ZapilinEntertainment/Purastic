using System;

namespace ZE.Purastic {
	public interface ICharacterController
	{
        public PlayableCharacter ActiveCharacter { get;}
        public Action<PlayableCharacter> OnCharacterChangedEvent { get; set; }
    }
}
