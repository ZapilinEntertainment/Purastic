using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public sealed class BlockHostsManager
	{
		private int _nextId = 1;
		private Dictionary<int, IBlocksHost> _blockHosts = new();

		public int Register(IBlocksHost host)
		{
			var id = _nextId++;
			_blockHosts.Add(id, host);
			return id;
		}
        public bool TryGetHost(int id, out IBlocksHost host) => _blockHosts.TryGetValue(id, out host);
        public void Unregister(IBlocksHost host)
		{
			_blockHosts.Remove(host.ID);
		}
		
	}
}
