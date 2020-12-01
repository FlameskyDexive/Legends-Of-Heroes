using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Planilo.BT.Custom {

  /// <summary>An example task node that debugs a blackboard variable.</summary>
  [CreateNodeMenu("BT/Action/DebugVariable")]
  public class DebugVariable : BTTaskNode {

    /// <summary>The target variable to debug.</summary>
    [Input] public BlackboardBaseNode _target = null;

    /// <summary>A reference to check whether the target is set or not.</summary>
    object _notSet = new object();

    /// <summary>Execute task.</summary>
	  protected override BTGraphResult InternalRun() {
      // Get value of variable
      object target = GetBlackboardValue<object>("_target", _target, _notSet);
      // Make sure target is set.
      if (target == _notSet) {
        return BTGraphResult.Failure;
      }
      // Only debug if the current game object is selected.
      GameObject go = BT.GetValue<GameObject>("GameObject");
      if (Selection.activeGameObject == go) {
        Debug.Log(string.Format("{0}: Result {1}", name, target));
      }
      // Return success.
      return BTGraphResult.Success;
    }
  }
}