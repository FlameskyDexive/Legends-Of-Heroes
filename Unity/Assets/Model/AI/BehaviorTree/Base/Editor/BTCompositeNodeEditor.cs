using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Planilo.BT {

  /// <summary>Custom editor for BTCompositeNode.</summary>
  [CustomNodeEditor(typeof(BTCompositeNode))]
  public class BTCompositeNodeEditor : BTBranchNodeEditor {

    /// <summary>Artificial port for creating new connections.</summary>
    NodePort _newChild;
    /// <summary>Reference to the composite node.</summary>
    BTCompositeNode _composite;

    /// <summary>Draw node's body GUI.</summary>
    public override void OnBodyGUI() {
      base.OnBodyGUI();

      _composite = target as BTCompositeNode;

      // Only work the GUI for the current exits and entries.
      int childCount = _composite.ChildCount;

      // Render children ports.
      EditorGUILayout.Space();

      for (int i = 0; i < childCount; i++) {
        BTConnection connection = _composite.GetChildConnection(i);
        NodePort port = target.GetOutputPort(connection.PortName);
        NodePort connected = port.Connection;

        if (connected == null) {
          _composite.RemoveChildConnection(i);
          i--;
          childCount--;
        } else {
          GUILayout.BeginHorizontal();
          EditorGUILayout.Space();
          NodeEditorGUILayout.PortField(new GUIContent((i + 1).ToString()), port, GUILayout.Width(50));
          GUILayout.EndHorizontal();
        }
      }

      // Check if we need to create new exit.
      _newChild = target.GetOutputPort("newChild");
      if (_newChild == null) {
        _newChild = target.AddInstanceOutput(typeof(BTConnection), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "newChild");
      }

      // If exit connection is not empty create new exit.
      if (_newChild.Connection != null) {
        _composite.AddChildConnection(_newChild.Connection);
        _newChild.Disconnect(_newChild.Connection);
      }

      GUILayout.BeginHorizontal();
      EditorGUILayout.Space();
      NodeEditorGUILayout.PortField(_newChild, GUILayout.Width(80));
      GUILayout.EndHorizontal();
    }
  }
}