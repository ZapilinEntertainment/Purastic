using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockStructureModule : SubcontainerModule
	{	

		private int _nextConnectionID = Utilities.GenerateInteger();
		private readonly ComplexResolver<PlacedBlocksListHandler, CuttingPlanesManager> _resolver;		
		private Dictionary<int, BlocksConnection> _connections = new();
		protected PlacedBlocksListHandler BlocksList => _resolver.Item1;
		protected CuttingPlanesManager CutPlanes => _resolver.Item2;	

		public System.Action<BlocksConnection> OnConnectionCreatedEvent;

		public IReadOnlyCollection<BlocksConnection> GetConnections() => _connections.Values;


        public BlockStructureModule(Container container) : base(container) 
		{
			_resolver = new(OnDependenciesReady, container);
			_resolver.CheckDependencies();
		}
		private void OnDependenciesReady() { }

		public void AddBlock(PlacedBlock baseBlock, FitElementStructureAddress fitInfo, VirtualBlock virtualBlock, ConnectedAndLockedPinsContainer pinsContainer, out PlacedBlock placedBlock)
		{

			var cutPlane = pinsContainer.BasementCutPlane;
			Vector2 cutPlanePoint = cutPlane.PlaneAddressToCutPlanePos(pinsContainer.BasementConnectedPins[0]);
			Vector3 contactPoint = baseBlock.FacePositionToModelPosition(cutPlanePoint, cutPlane.Face);

			BlockFaceDirection newBlockContactFace = baseBlock.Rotation.TransformDirection(fitInfo.ContactFace.Inverse());

            placedBlock = BlocksList.RegisterBlock(virtualBlock);
			var cutPlaneCoord = CutPlanes.GetCutPlaneCoordinate(placedBlock, newBlockContactFace);
            RegisterConnection(baseBlock, placedBlock, CutPlanes.GetOrCreateCutPlane(cutPlaneCoord), pinsContainer);
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
