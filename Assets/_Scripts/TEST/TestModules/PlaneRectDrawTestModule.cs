using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class PlaneRectDrawTestModule : PinDefineTestModule
	{
        private bool _isReady = false;
        private RectDrawer _rectDrawer = null;
        private BlockHostsManager _hostsManager;
        protected override bool IsReady => base.IsReady & _isReady;

        private void Start()
        {
            _hostsManager = ServiceLocatorObject.Get<BlockHostsManager>();
            _isReady = true;
        }
        protected override void OnPinFound(RaycastHit rh)
        {
            base.OnPinFound(rh);
            if (_hostsManager.TryGetHost(PositionInfo.BlockHostID, out var host))
            {
                int cutPlaneiD = PositionInfo.StructureAddress.CutPlaneID;
                if (host.CutPlanesDataProvider.GetCuttingPlane(cutPlaneiD).TryGetFitPlane(PositionInfo.StructureAddress.BlockID, PositionInfo.StructureAddress.PlaneAddress.SubPlaneId, out var dataprovider))
                {
                    var rect = dataprovider.ToRectangle();
                    _rectDrawer = RectDrawer.CreateRectDrawer(host, cutPlaneiD, rect, Color.cyan, 0f);
                }
            }            
        }
        protected override void OnFixedUpdate(RaycastHit rh)
        {
            base.OnFixedUpdate(rh);
            if (!PinFound)
            {
                if (_hostsManager.TryGetHost(PositionInfo.BlockHostID, out var host))
                {
                    int cutPlaneiD = PositionInfo.StructureAddress.CutPlaneID;
                    if (host.TryGetBlock(PositionInfo.StructureAddress.BlockID, out var block))
                        _rectDrawer = RectDrawer.CreateRectDrawer(host, cutPlaneiD, block, Color.red, 0f);
                    else Debug.Log("block not found by id");
                }
            }
        }
        protected override void OnDrawAnyGizmos()
        {
            _rectDrawer?.DrawRect();
        }
    }
}
