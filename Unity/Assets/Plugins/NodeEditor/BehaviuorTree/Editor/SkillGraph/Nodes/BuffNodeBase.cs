using ET;
using GraphProcessor;
using UnityEditor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    public class BuffNodeBase: BaseNode
    {
        [Input("InputBuff", allowMultiple = true)]
        [HideInInspector]
        public BuffNodeBase PrevNode;
        
        [Output("OutputBuff", allowMultiple = true)]
        [HideInInspector]
        public BuffNodeBase NextNode;

        public override Color color => Color.green;

        public virtual void AutoAddLinkedBuffs()
        {
            
        }
        
        public virtual BuffNodeDataBase GetBuffNodeData()
        {
            return null;
        }
    }
}
