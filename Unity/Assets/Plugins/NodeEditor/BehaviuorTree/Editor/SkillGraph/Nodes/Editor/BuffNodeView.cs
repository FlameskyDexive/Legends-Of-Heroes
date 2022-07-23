using ET;
using GraphProcessor;
using UnityEngine.UIElements;

namespace Plugins.NodeEditor
{
    [NodeCustomEditor(typeof(BuffNodeBase))]
    public class BuffNodeView: BaseNodeView
    {
        public override void Enable()
        {
            BuffNodeDataBase nodeDataBase = (this.nodeTarget as BuffNodeBase).GetBuffNodeData();
            TextField textField = new TextField();
            if (nodeDataBase is NormalBuffNodeData normalBuffNodeData)
            {
                textField.value = normalBuffNodeData.BuffDes;
                textField.RegisterValueChangedCallback((changedDes) => { normalBuffNodeData.BuffDes = changedDes.newValue; });
            }
            else if(nodeDataBase is SkillDesNodeData skillDesNodeData)
            {
                textField.value = skillDesNodeData.SkillName;
            }
            textField.style.marginTop = 4;
            textField.style.marginBottom = 4;

            controlsContainer.Add(textField);
        }
    }
}
