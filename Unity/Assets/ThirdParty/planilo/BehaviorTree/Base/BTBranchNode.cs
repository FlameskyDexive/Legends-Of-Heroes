using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {

  /// <summary>Base class for nodes at the branch level of the behavior tree.</summary>
  /// <remarks>Inherits from <see cref="Planilo.BT.BTNode"/></remarks>
  public abstract class BTBranchNode : BTNode {

    /// <summary>Gets whether the node is the root of the behavior tree. Sets or unset the node as the root.</summary>
    public bool IsRoot {
      get { return _isRoot; }
      set {
        BTGraph bt = (graph as BTGraph);
        if (value) {
          // Set root in graph.
          bt.SetRoot(this);
          // Clear parent port.
          NodePort port = GetInputPort("_parent");
          port.Disconnect(port.Connection);
        } else {
          // Unset root in graph.
          bt.UnsetRoot(this);
        }
        // Set flag value.
        _isRoot = value;
      }
    }

    /// <summary>Is the node the root of the tree?</summary>
    [SerializeField][HideInInspector] bool _isRoot;

    /// <summary>Set this node as the behavior tree root.</summary>
    [ContextMenu("Set as root")]
    public void SetAsRoot() {
      IsRoot = true;
    }
  }
}