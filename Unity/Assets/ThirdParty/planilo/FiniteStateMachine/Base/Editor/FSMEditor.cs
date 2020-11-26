using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Planilo.FSM {

  /// <summary>A custom editor for FSMGraph.</summary>
  [CustomNodeGraphEditor(typeof(FSMGraph))]
  public class FSMEditor : AIGraphEditor {

    /// <summary>Get available types of nodes to be created.</summary>
    public override string GetNodeMenuName(System.Type type) {
      // Make sure type is a valid instance.
      bool isValid =
        type.IsSubclassOf(typeof(FSMStateNode)) ||
        type.IsSubclassOf(typeof(BlackboardBaseNode)) ||
        type.IsSubclassOf(typeof(FSMCheckNode));

      if (!isValid) { return null; }
      // Return name as normal
      else return base.GetNodeMenuName(type).Replace("FSM/", "");
    }
  }
}