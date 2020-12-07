using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Planilo.BT {

  /// <summary>Custom editor for BTNode.</summary>
  [CustomNodeEditor(typeof(BTNode))]
  public class BTNodeEditor : NodeEditor {

    /// <summary>The color to be used when node is running.</summary>
    Color _runningColor = Color.yellow;

    /// <summary>Define what color to use on the node.</summary>
    public override Color GetTint() {
      // Check if there is an active game object with a bt runner.
      AIGraphRunner runner = Selection.activeGameObject != null ?
        Selection.activeGameObject.GetComponent<AIGraphRunner>() : null;

      if (runner == null) { return base.GetTint(); }
      // Check if it is running.
      BTNode node = target as BTNode;
      float? runTime = (float?) runner.GetFromBlackboard("BTRunTime" + node.GetInstanceID());

      return runTime.HasValue && runTime.Value == Time.time ? _runningColor : base.GetTint();
    }
  }
}