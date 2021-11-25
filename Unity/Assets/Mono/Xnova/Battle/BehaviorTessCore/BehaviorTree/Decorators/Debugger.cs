using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;

namespace Planilo.BT {

	/// <summary>A decorator node debugging the output of the child node.</summary>
  [CreateNodeMenu("BT/Decorator/Debugger")]
  public class Debugger : BTDecoratorNode {

    /// <summary>Execute decorator.</summary>
	  protected override BTGraphResult InternalRun() {
      // Set default exit value.
      BTGraphResult result = BTGraphResult.Success;
      // Execute all childs, exit when one succeeds.
      if (_child != null) {
        result = _child.Run();
      }
      // Only debug if the current game object is selected.
      GameObject go = BT.GetValue<GameObject>("GameObject");
      if (Selection.activeGameObject == go) {
        Debug.Log(string.Format("{0}: Result {1}", name, result));
      }
      // Return negation of result.
      return result;
    }
  }
}