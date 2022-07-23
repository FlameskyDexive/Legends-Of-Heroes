using ET;
using GraphProcessor;

namespace Plugins.NodeEditor
{
    [NodeMenuItem("NPBehave行为树/Decorator/BlackboardCondition", typeof (NPBehaveGraph))]
    [NodeMenuItem("NPBehave行为树/Decorator/BlackboardCondition", typeof (SkillGraph))]
    public class NP_BlackboardConditionNode: NP_DecoratorNodeBase
    {
        public override string name => "黑板条件结点";

        public NP_BlackboardConditionNodeData NP_BlackboardConditionNodeData =
                new NP_BlackboardConditionNodeData { NodeDes = "黑板条件结点" };

        public override NP_NodeDataBase NP_GetNodeData()
        {
            return NP_BlackboardConditionNodeData;
        }
    }
}
