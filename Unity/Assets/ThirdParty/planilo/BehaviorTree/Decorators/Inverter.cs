using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {

	/// <summary>A decorator node inverting the output of the child node.</summary>
  [CreateNodeMenu("BT/Decorator/Inverter")]
  public class Inverter : BTDecoratorNode {

    /// <summary>Execute decorator.</summary>
	  protected override BTGraphResult InternalRun() {
      // Set default exit value.
      BTGraphResult result = BTGraphResult.Success;
      // Execute child.
      if (_child != null) {
        result = _child.Run();
      }
      // Return negation of result.
      return result.Invert();
    }
  }
}