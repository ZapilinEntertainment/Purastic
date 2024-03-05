using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
    public class PlacedBlock // must be class, not struct
    {        
        public readonly int ID;
        public readonly Vector3 LocalPosition;
        public readonly PlacedBlockRotation Rotation;
        public readonly BlockProperties Properties;

        public PlacedBlock(int id,Vector3 localPosition,  BlockProperties properties)
        {
            this.ID = id;
            this.LocalPosition = localPosition;
            this.Properties = properties;
        }
        public PlacedBlock(int id, Vector3 localPos, BlockProperties properties, PlacedBlockRotation rotation) : this(id,localPos, properties)
        {
            this.Rotation = rotation;
        }

        
        public FitPosition GetPinPosition(Vector3 pos) // local pos in construction platform space
        {
            Vector3 size = Properties.ModelSize;
            Vector3 localPos = Matrix4x4.TRS(LocalPosition, Rotation.Quaternion, Vector3.one) * pos; // local pos in brick coordinates
            FitPlaneDirection direction;
            sbyte Yquadrant = 0;

            if (localPos.y >= size.y) Yquadrant = 1; else if (localPos.y <= 0f) Yquadrant = -1;
            if (Yquadrant == 0)
            {
                sbyte Xquadrant = 0;
                if (localPos.x >= size.x * 0.5f) Xquadrant = 1; else if (localPos.x <= -size.x * 0.5f) Xquadrant = -1;
                if (Xquadrant == 0)
                {
                    sbyte Zquadrant = 0;
                    if (localPos.z >= size.z * 0.5f) Zquadrant = 1; else if (localPos.z <= -size.z * 0.5f) Zquadrant = -1;
                    if (Zquadrant == 1) direction = FitPlaneDirection.Forward;
                    else
                    {
                        if (Zquadrant != 0) direction = FitPlaneDirection.Back;
                        else direction = FitPlaneDirection.Undefined;
                    }
                }
                else
                {
                    if (Xquadrant == 1) direction = FitPlaneDirection.Right; else direction = FitPlaneDirection.Left;
                }
            }
            else
            {
                if (Yquadrant == 1) direction = FitPlaneDirection.Up;
                else direction = FitPlaneDirection.Down;
            }

            var planesHost = FitPlanesConfigsDepot.LoadConfig(Properties.FitPlanesHash);
            if (planesHost.TryGetFitPosition(direction, localPos, out var fitPos)) return fitPos;
            else return new FitPosition(pos);
        }
        public bool TryFormFitInfo(FitPosition position, BlockProperties connectingBlock, out FitInfo info)
        {
            return FitPlanesConnector.TryConnect(position.PinIndex,Vector2Byte.zero, Properties.FitPlanesHash, connectingBlock.FitPlanesHash, out info);
        }
    }
}
