using UnityEngine;

namespace ET.Client
{
    public static class LayerNames
    {
        public const string UI = "UI";
        public const string UNIT = "Unit";
        public const string MAP = "Map";
        public const string DEFAULT = "Default";
        public const string HIDDEN = "Hidden";

        public static int GetLayerInt(string name)
        {
            return LayerMask.NameToLayer(name);
        }

        public static string GetLayerStr(int name)
        {
            return LayerMask.LayerToName(name);
        }
    }
}
