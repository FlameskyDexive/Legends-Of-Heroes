using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {

  /// <summary>Behavior tree node that will execute a sub behavior tree.</summary>
  [CreateNodeMenu("BT/Subtree")]
  [NodeTint("#6b562e")]
  public class BTSubtreeNode : BTTaskNode {

    /// <summary>Subtree that will be run.</summary>
    [SerializeField] protected BTGraph _tree;

    /// <summary>Set the subtree to be executed.</summary>
    public BTGraph Tree {
      set {
        _tree = value;
      }
    }

    /// <summary>Execute the subtree node.</summary>
    protected override BTGraphResult InternalRun() {
      // Check if machine has been initialized.
      AIBlackboard blackboard = BT.Blackboard;
      object initialized = null;
      blackboard.variables.TryGetValue("Init" + GetInstanceID(), out initialized);
      // If machine hasn't been initialized it is time to do so.
      if (initialized == null) {
        _tree.Init(blackboard);
        blackboard.variables["Init" + GetInstanceID()] = true;
      }

      // Execute and return isolating running statuses.
      BTGraphResult result = _tree.Run(blackboard) as BTGraphResult;
      return result.IsRunning ? BTGraphResult.Success : result;
    }
  }
}