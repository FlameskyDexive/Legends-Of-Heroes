using ET;
using GraphProcessor;

namespace Plugins.NodeEditor
{
    public abstract class NP_NodeBase : BaseNode
    {
        /// <summary>
        /// 层级，用于自动排版
        /// </summary>
        public int Level;
        
        public virtual NP_NodeDataBase NP_GetNodeData()
        {
            return null;
        }
    }
}
