using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {

	/// <summary>A decorator node forcing success on any output of the child node.</summary>
  [CreateNodeMenu("BT/Decorator/Succeeder")]
  public class Succeeder : BTDecoratorNode {

    /// <summary>Execute decorator.</summary>
	  protected override BTGraphResult InternalRun() {
      // Execute all childs, exit when one succeeds.
      if (_child != null) {
        _child.Run();
      }
      // Always return success.
      return BTGraphResult.Success;
    }
  }
}