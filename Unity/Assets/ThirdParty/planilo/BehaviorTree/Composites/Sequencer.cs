using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planilo.BT {

	/// <summary>A composite node implementing a sequencer (stop on child failure).</summary>
  [CreateNodeMenu("BT/Composite/Sequencer")]
  public class Sequencer : BTCompositeNode {

    /// <summary>When to break the loop of children execution.</summary>
    protected override bool ShouldBreakLoop(BTGraphResult result) {
      return result.IsFailure;
    }
  }
}