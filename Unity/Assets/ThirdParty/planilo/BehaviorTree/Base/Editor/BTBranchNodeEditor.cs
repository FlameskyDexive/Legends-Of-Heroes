using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Planilo.BT {

  /// <summary>Custom editor for BTBranchNode.</summary>
  [CustomNodeEditor(typeof(BTBranchNode))]
  public class BTBranchNodeEditor : BTNodeEditor {

    /// <summary>Reference to the branch node.</summary>
    BTBranchNode _branch;

    /// <summary>Draw node's body GUI.</summary>
    public override void OnBodyGUI() {
      base.OnBodyGUI();

      _branch = target as BTBranchNode;

      // Show parent port only if node is not root.
      if (!_branch.IsRoot) {
        GUILayout.BeginHorizontal();
        NodePort port = _branch.GetInputPort("_parent");
        NodeEditorGUILayout.PortField(port, GUILayout.Width(60));
        GUILayout.EndHorizontal();
      }
    }
  }
}