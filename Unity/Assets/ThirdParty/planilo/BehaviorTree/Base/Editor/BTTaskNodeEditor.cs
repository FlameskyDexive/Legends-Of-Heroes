using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Planilo.BT {

  /// <summary>Custom editor for BTTaskNode.</summary>
  [CustomNodeEditor(typeof(BTTaskNode))]
  public class BTTaskNodeEditor : BTNodeEditor {

    /// <summary>Draw node's body GUI.</summary>
    BTTaskNode _taskNode;
    public override void OnBodyGUI() {
      base.OnBodyGUI();

      // Show parent port.
      GUILayout.BeginHorizontal();
      NodePort port = target.GetInputPort("_parent");
      NodeEditorGUILayout.PortField(port, GUILayout.Width(60));
      GUILayout.EndHorizontal();
    }
  }
}