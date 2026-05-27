using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class RedDotComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<string, ListComponent<string>> RedDotNodeParentsDict = new Dictionary<string, ListComponent<string>>();
        public readonly HashSet<string> RedDotNodeNeedShowSet = new HashSet<string>();
        public readonly Dictionary<string, int> RetainViewCount = new Dictionary<string, int>();
        public readonly Dictionary<string, string> ToParentDict = new Dictionary<string, string>();
        public readonly Dictionary<string, int> RedDotNodeRetainCount = new Dictionary<string, int>();
        public readonly Dictionary<string, RedDotMonoView> RedDotMonoViewDict = new Dictionary<string, RedDotMonoView>();
    }
}
