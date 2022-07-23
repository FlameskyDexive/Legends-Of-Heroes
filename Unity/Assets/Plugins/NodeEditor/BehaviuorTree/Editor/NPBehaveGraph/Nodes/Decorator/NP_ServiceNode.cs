using ET;
using GraphProcessor;
using Sirenix.OdinInspector;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("NPBehave行为树/Decorator/Service", typeof (NPBehaveGraph))]
    [NodeMenuItem("NPBehave行为树/Decorator/Service", typeof (SkillGraph))]
    public class NP_ServiceNode: NP_DecoratorNodeBase
    {
        public override string name => "服务结点";

        [BoxGroup("服务结点数据")]
        [HideReferenceObjectPicker]
        [HideLabel]
        public NP_ServiceNodeData NP_ServiceNodeData = new NP_ServiceNodeData { };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_ServiceNodeData;
        }
    }
}
