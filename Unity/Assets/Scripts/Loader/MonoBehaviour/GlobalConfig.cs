using UnityEngine;

namespace ET
{
    public enum CodeMode
    {
        Client = 1,
        Server = 2,
        ClientServer = 3,
    }
    
    public enum BuildType
    {
        None,
        Debug,
        Release,
    }
    
    [CreateAssetMenu(menuName = "ET/CreateGlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig: ScriptableObject
    {
        public static GlobalConfig Instance;
        
        public CodeMode CodeMode;
        
        public BuildType BuildType;

        public AppType AppType;

        public int ModelVersion = 1;

        public int HotFixVersion = 1;
    }
}