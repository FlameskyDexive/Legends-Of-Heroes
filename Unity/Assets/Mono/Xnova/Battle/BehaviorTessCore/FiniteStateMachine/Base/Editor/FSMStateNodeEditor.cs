using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Planilo.FSM {
  /// <summary>A custom node editor for state nodes.</summary>
  [CustomNodeEditor(typeof(FSMStateNode))]
  public class FSMStateNodeEditor : NodeEditor {

    /// <summary>Virtual node port for new entries.</summary>
    NodePort _entry;
    /// <summary>Virtual node port for new exits.</summary>
    NodePort _exit;
    /// <summary>Reference to the target state.</summary>
    FSMStateNode _state;

    /// <summary>Draw node's body.</summary>
    public override void OnBodyGUI() {
      base.OnBodyGUI();
      OnTransitionsGUI();
    }

    /// <summary>Draw transition ports.</summary>
    protected virtual void OnTransitionsGUI() {
      _state = target as FSMStateNode;

      // Only work the GUI for the current exits and entries.
      int exitCount = _state.ExitsCount;
      int entryCount = _state.EntriesCount;

      // Check if we need to create new entry.
      _entry = target.GetInputPort("entry");
      if (_entry == null) {
        _entry = target.AddInstanceInput(typeof(FSMConnection), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "entry");
      }

      // If entry connection is not empty create new entry.
      if (_entry.Connection != null) {
        _state.AddEntryConnection(_entry.Connection);
        _entry.Disconnect(_entry.Connection);
      }

      // Check if we need to create new exit.
      _exit = target.GetOutputPort("exit");
      if (_exit == null) {
        _exit = target.AddInstanceOutput(typeof(FSMConnection), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "exit");
      }

      // If exit connection is not empty create new exit.
      if (_exit.Connection != null) {
        _state.AddExitConnection(_exit.Connection);
        _exit.Disconnect(_exit.Connection);
      }

      GUILayout.BeginHorizontal();
      NodeEditorGUILayout.PortField(_entry, GUILayout.Width(50));
      EditorGUILayout.Space();
      NodeEditorGUILayout.PortField(_exit, GUILayout.Width(50));
      GUILayout.EndHorizontal();

      EditorGUILayout.Space();
      for (int i = 0; i < entryCount; i++) {
        FSMConnection connection = _state.GetEntryConnection(i);
        NodePort port = target.GetInputPort(connection.PortName);
        NodePort connected = port.Connection;

        if (connected == null) {
          _state.RemoveEntryConnection(i);
          i--;
          entryCount--;
        } else {
          GUILayout.BeginHorizontal();
          NodeEditorGUILayout.PortField(new GUIContent(), port, GUILayout.Width(-4));
          EditorGUILayout.LabelField(string.Format("> {0}", connected.node.name));
          GUILayout.EndHorizontal();
        }
      }

      EditorGUILayout.Space();

      for (int i = 0; i < exitCount; i++) {
        FSMConnection connection = _state.GetExitConnection(i);
        NodePort port = target.GetOutputPort(connection.PortName);
        NodePort connected = port.Connection;

        if (connected == null) {
          _state.RemoveExitConnection(i);
          i--;
          exitCount--;
        } else {
          GUILayout.BeginHorizontal();
          EditorGUILayout.Space();
          NodeEditorGUILayout.PortField(new GUIContent(), port, GUILayout.Width(50));
          GUILayout.EndHorizontal();
        }
      }
    }

    /// <summary>Get tint of the target node.</summary>
    public override Color GetTint() {
      // Check if there is an active game object.
      if (Selection.activeGameObject == null) { return base.GetTint(); }
      // Try to get active state from FSM runner.
      AIGraphRunner runner = Selection.activeGameObject.GetComponent<AIGraphRunner>();
      if (runner == null || _state == null) { return base.GetTint(); }
      // Check if it is running.
      FSMGraph graph = _state.graph as FSMGraph;
      FSMStateNode current =
        runner.GetFromBlackboard("CurrentState" + graph.GetInstanceID()) as FSMStateNode;
      return current == _state ? Color.yellow : base.GetTint();
    }
  }
}