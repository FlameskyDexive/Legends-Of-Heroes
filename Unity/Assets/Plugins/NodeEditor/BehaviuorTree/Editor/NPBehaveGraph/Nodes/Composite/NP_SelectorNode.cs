using ET;
using GraphProcessor;
using Sirenix.OdinInspector;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("NPBehave行为树/Composite/Selector", typeof (NPBehaveGraph))]
    [NodeMenuItem("NPBehave行为树/Composite/Selector", typeof (SkillGraph))]
    public class NP_SelectorNode: NP_CompositeNodeBase
    {
        public override string name => "选择结点";

        [BoxGroup("Selector结点数据")]
        [HideReferenceObjectPicker]
        [HideLabel]
        public NP_SelectorNodeData NP_SelectorNodeData = new NP_SelectorNodeData { NodeDes = "选择组合器"};

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_SelectorNodeData;
        }
    }
}
