using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Planilo.BT {

  /// <summary>Custom editor for BTDecoratorNode.</summary>
  [CustomNodeEditor(typeof(BTDecoratorNode))]
  public class BTDecoratorNodeEditor : BTBranchNodeEditor {

    /// <summary>Reference to the decorator node.</summary>
    BTDecoratorNode _decorator = null;
    /// <summary>Reference to the node connected to the decorator.</summary>
    Node _connection = null;
    /// <summary>Reference to the child port.</summary>
    NodePort _child = null;

    /// <summary>Draw node's body GUI.</summary>
    public override void OnBodyGUI() {
      base.OnBodyGUI();

      _decorator = target as BTDecoratorNode;

      // Check if we need to create new exit.
      _child = target.GetOutputPort("_child");
      if (_child == null) {
        _child = target.AddInstanceOutput(typeof(BTConnection), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "_child");
      }

      // Output port field.
      GUILayout.BeginHorizontal();
      EditorGUILayout.Space();
      NodeEditorGUILayout.PortField(_child, GUILayout.Width(60));
      GUILayout.EndHorizontal();

      // Update connection values.
      if (_child.Connection == null) {
        _connection = null;
        _decorator.Child = null;
      } else if (_child.Connection.node != _connection) {
        _connection = _child.Connection.node;
        _decorator.Child = new BTConnection(_decorator, _child.fieldName);
      }
    }
  }
}