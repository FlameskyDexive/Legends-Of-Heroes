using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo {

  /// <summary>A base class for nodes that provide blackboard data to other nodes.</summary>
  [CreateNodeMenu("")]
  [NodeTint("#6b2e38")]
  public abstract class BlackboardBaseNode : Node {

    /// <summary>The current value of the blackboard variable.</summary>
    public abstract object GetValue();
  }
}