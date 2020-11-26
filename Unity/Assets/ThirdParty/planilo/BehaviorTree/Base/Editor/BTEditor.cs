using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Planilo.BT {

  /// <summary>Custom graph editor for BTGraph.</summary>
  [CustomNodeGraphEditor(typeof(BTGraph))]
  public class BTEditor : AIGraphEditor {

    /// <summary>Show available nodes to be created for the graph.</summary>
    public override string GetNodeMenuName(System.Type type) {
      // Make sure type is a valid instance.
      if (!type.IsSubclassOf(typeof(BTNode)) && !type.IsSubclassOf(typeof(BlackboardBaseNode))) { return null; }
      // Return name as normal
      return base.GetNodeMenuName(type).Replace("BT/", "");
    }
  }
}