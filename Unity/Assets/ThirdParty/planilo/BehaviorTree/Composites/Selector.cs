using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planilo.BT {

	/// <summary>A composite node implementing a selector (stop on child success).</summary>
  [CreateNodeMenu("BT/Composite/Selector")]
  public class Selector : BTCompositeNode {

    /// <summary>When to break the loop of children execution.</summary>
	  protected override bool ShouldBreakLoop(BTGraphResult result) {
      return result.IsSuccess;
    }
  }
}