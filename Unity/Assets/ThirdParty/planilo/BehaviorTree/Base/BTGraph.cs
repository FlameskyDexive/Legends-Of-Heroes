using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {

  /// <summary>Class to represent a behavior tree as an AIGraph.</sumamry>
  [CreateAssetMenu(fileName = "BehaviorTree", menuName = "BT/BehaviorTree", order = 1)]
  public class BTGraph : AIGraph {

    /// <summary>Root node of the behavior tree.</summary>
    [SerializeField] private BTBranchNode root;

    /// <summary>Sets the root node of the behavior tree.</summary>
    public void SetRoot(BTBranchNode node) {
      // Unset previous root.
      if (root != null) {
        root.IsRoot = false;
      }
      // Set new root.
      root = node;
    }

    /// <summary>Unsets the root node of the behavior tree if the given node matches the root.</summary>
    public void UnsetRoot(BTBranchNode node) {
      // Only unset when the state passed is the same as the current one.
      if (node == root) {
        root = null;
      }
    }

    /// <summary>Initialize the behavior tree and the blackboard.</summary>
    public override void Init(AIBlackboard blackboard) {
      // Save blackboard while executing.
      _blackboard = blackboard;
      // Init all nodes.
      foreach (Node node in nodes) {
        BTNode btNode = node as BTNode;
        // If node is of BTNode type prepare it for execution.
        if (btNode != null) {
          btNode.Prepare();
        }
      }
    }

    /// <summary>Execute the behavior tree in the blackboard.</summary>
    public override AIGraphResult Run(AIBlackboard blackboard) {
      // Save blackboard while executing.
      _blackboard = blackboard;
      // Execute root.
      BTGraphResult result = root.Run();
      // Nullify references to the blackboard.
      _blackboard = null;
      // Return result.
      return result;
    }
  }
}