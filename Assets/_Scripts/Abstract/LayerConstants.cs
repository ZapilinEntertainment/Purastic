using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.Purastic {

    public enum CustomLayermask : byte { Default, GroundCast, BlockPlaceCast }
    public static class CustomLayermaskExtension { 
        public static int ToInt(this CustomLayermask layermask) => LayerConstants.GetCustomLayermask(layermask);
    }
    public enum DefinedLayer : byte { Default, Collectable, Player, Terrain, Pinplane }
    public static class LayerConstants
	{
        private static Dictionary<CustomLayermask, int> _customLayermasks = new Dictionary<CustomLayermask, int>();
        private static Dictionary<DefinedLayer, int> _definedLayers = new Dictionary<DefinedLayer, int>();
        public const string DEFAULT_LAYERNAME = "Default", COLLECTABLE_LAYERNAME = "Collectable", PLAYER_LAYERNAME = "Player", 
            TERRAIN_LAYERNAME = "Terrain", PINPLANE_LAYERNAME = "Pinplane";

        public static int GetDefinedLayer(DefinedLayer definedLayer)
        {
            int layer = 0;
            if (!_definedLayers.TryGetValue(definedLayer, out layer))
            {
                string layerName;
                switch (definedLayer)
                {
                    case DefinedLayer.Pinplane: layerName = PINPLANE_LAYERNAME; break;
                    case DefinedLayer.Player: layerName = PLAYER_LAYERNAME; break;
                    case DefinedLayer.Collectable: layerName = COLLECTABLE_LAYERNAME; break;
                    case DefinedLayer.Terrain: layerName = TERRAIN_LAYERNAME; break;
                    default: layerName = DEFAULT_LAYERNAME; break;
                }
                layer = LayerMask.NameToLayer(layerName);
                _definedLayers.Add(definedLayer, layer);
            }
            return layer;
        }
        public static int GetCustomLayermask(CustomLayermask customLayer)
        {
            if (!_customLayermasks.TryGetValue(customLayer, out int value))
            {
                switch (customLayer)
                {
                    case CustomLayermask.BlockPlaceCast: value = LayerMask.GetMask(PINPLANE_LAYERNAME); break;
                    case CustomLayermask.GroundCast: value = LayerMask.GetMask(DEFAULT_LAYERNAME, TERRAIN_LAYERNAME); break;
                    default: value = LayerMask.GetMask(DEFAULT_LAYERNAME); break;
                }
                _customLayermasks.Add(customLayer, value);
            }
            return value;
        }
    }
}
