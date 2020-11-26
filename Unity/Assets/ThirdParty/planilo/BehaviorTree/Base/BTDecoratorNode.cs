using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {
  /// <summary>Base class for decorator nodes in the Behavior Tree.</summary>
  [CreateNodeMenu("")]
  [NodeTint("#2e6b6b")]
  public abstract class BTDecoratorNode : BTBranchNode {

    /// <summary>Gets/Sets the unique child of this node.</summary>
    public BTConnection Child {
      get { return _child; }
      set { _child = value; }
    }

    /// <summary>The child connection.</summary>
    [SerializeField][HideInInspector] protected BTConnection _child;

    /// <summary>Initializes this and the child node for execution.</summary>
    protected override void Init() {
      base.Init();
      // Initialize child connection.
      if (_child != null) {
        _child.Init();
      }
    }
  }
}