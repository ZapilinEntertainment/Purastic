using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockStructureModule
	{	
		private int _nextConnectionID = Utilities.GenerateInteger();
		private readonly PlacedBlocksListHandler _blockList;		
		private Dictionary<int, BlocksConnection> _connections = new();
		public System.Action<BlocksConnection> OnConnectionCreatedEvent;

		public IReadOnlyCollection<BlocksConnection> GetConnections() => _connections.Values;


        public BlockStructureModule(PlacedBlocksListHandler list)
		{
			_blockList= list;
		}

		public void AddBlock(PlacedBlock baseBlock, FitElementStructureAddress fitInfo, PlacingBlockInfo placingBlockInfo, ConnectedAndLockedPinsContainer pinsContainer, out PlacedBlock placedBlock)
		{
			Vector3 basementPoint = pinsContainer.BasementCutPlane.PlaneAddressToLocalPos(pinsContainer.BasementConnectedPins[0]);
			BlockFaceDirection newBlockContactFace = baseBlock.Rotation.TransformDirection(fitInfo.ContactFace);
            Vector3 newBlockLocalPos = placingBlockInfo.CalculateLocalPosition(basementPoint, pinsContainer.NewBlockConnectedPins[0], newBlockContactFace);

            placedBlock = _blockList.RegisterBlock(placingBlockInfo, newBlockLocalPos);
            RegisterConnection(baseBlock, placedBlock, , pinsContainer);
        }

		private void RegisterConnection(PlacedBlock blockA, PlacedBlock blockB, ICuttingPlane newBlockCutPlane, ConnectedAndLockedPinsContainer pinsContainer)
		{
			int id = _nextConnectionID++;
			var connection = new BlocksConnection(id, blockA, blockB, newBlockCutPlane, pinsContainer);
            _connections.Add(id, connection);
			OnConnectionCreatedEvent?.Invoke(connection);
        }
	}
}
