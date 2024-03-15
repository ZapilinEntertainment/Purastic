using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public class BlockModel : MonoBehaviour, IEquippable, IPlaceable, IPoolableModel
	{
        private int _blockId = -1;
		private BlockProperties _properties;
        private IBlocksHost _blocksHost;
        private Dictionary<FitElementPlaneAddress, GameObject> _pinModels = new();
		public bool IsPlaceable => true;
        public bool IsVisible { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
        public GameObject ModelObject => gameObject;

        public BlockProperties GetBlockProperty() => _properties;


        private CuttingPlanePosition FormCutPlaneCoord(FitPlaneConfig config)
        {
            var face = config.Face;
            //Debug.Log($"{config.Face}:{config.ZeroPos}");
            Vector3 zeroPos = _blocksHost.InverseTransformPosition(transform.TransformPoint(config.ZeroPos));
            return new CuttingPlanePosition(face, Vector3.Dot(zeroPos, face.Normal));
        }
        public void OnEquip(Transform handPoint)
        {
            transform.SetParent(handPoint, false);
			transform.localPosition = new Vector3(0f, _properties.ModelSize.z * 0.5f, _properties.ModelSize.y * 0.5f);
			transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
        }

        public void InitializeModel(Dictionary<FitElementPlaneAddress, GameObject> pins )
        {
            _pinModels= pins;
        }
        public void Setup(BlockProperties block, Material visualMaterial) {
			_properties = block;
			SetDrawMaterial(visualMaterial);
		}
        public void AssignHost(IBlocksHost host, PlacedBlock block)
        {
            _blocksHost = host;
            _blockId = block.ID;
            var cutPlanesDataProvider = _blocksHost.CutPlanesDataProvider;
            if (cutPlanesDataProvider != null)
            {
                var planes = _properties.GetPlanesList().Planes;
                int planesCount = planes.Count;
                var lockZones = new CuttingPlaneLockZone[planesCount]; 
                BitArray lockZonesMask = new BitArray(planesCount, false);
                for (int i = 0; i < planesCount; i++)
                {
                    var plane = planes[i];
                    var coordinate = FormCutPlaneCoord(planes[i]);
                    if (cutPlanesDataProvider.TryGetLockZone(coordinate, out var lockZone))
                    {
                        lockZones[i] = lockZone;
                        lockZonesMask[i] = true;
                    }
                    else lockZonesMask[i] = false;
                }

                
                foreach (var pinModel in _pinModels)
                {
                    int planeId = pinModel.Key.SubPlaneId;
                    if (lockZonesMask[planeId]) CheckPinLockStatus(pinModel.Value,pinModel.Key, lockZones[planeId]);
                }

                
            }
        }
        void CheckPinLockStatus(GameObject pin, FitElementPlaneAddress address, CuttingPlaneLockZone lockZone)
        {
            pin.SetActive(lockZone == null || !lockZone.Contains(_blockId, address));
        }

        public void SetDrawMaterial(Material material)
		{
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.sharedMaterial = material;
            }
        }

        public void SetPoint(Vector3 pos, Quaternion rotation)
        {
            transform.SetPositionAndRotation(pos, rotation);
        }

        public void Dispose()
        {
            _blocksHost = null;
            Destroy(gameObject);
        }

        public void OnSpawnedFromPool() => SetVisibility(true);
        public void OnReturnedToPool()
        {
            _blocksHost = null;
            SetVisibility(false);
        }
        public void OnPinsLocked(CuttingPlanePosition coordinate, CuttingPlaneLockZone lockZone)
        {            
            var planes = _properties.GetPlanesList().Planes;
            int affectedPlane = -1;
            for (int i = 0; i < planes.Count; i++)
            {
                var plane = planes[i];
                var planeCoord = FormCutPlaneCoord(plane);
                if (coordinate == planeCoord )
                {
                    affectedPlane = i;
                    break;
                }
            }
            if (affectedPlane != -1)
            {                
                foreach (var model in _pinModels)
                {
                    if (model.Key.SubPlaneId == affectedPlane) CheckPinLockStatus(model.Value,model.Key, lockZone);
                }
            }
        }
        public void SetVisibility(bool x) => ModelObject.SetActive(x);
    }
}
