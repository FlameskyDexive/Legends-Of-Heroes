using System.Collections;
using System.Collections.Generic;
using Planilo.BT;
using UnityEngine;

namespace Planilo.FSM {
  [CreateNodeMenu("FSM/State/BehaviorTree")]
  public class FSMBehaviorTreeNode : FSMStateNode {

    [SerializeField] BTGraph _tree;

    public override void Run() {
      _tree.Run((graph as FSMGraph).Blackboard);
    }

    public override void Enter() {
      // Check if machine has been initialized.
      AIBlackboard blackboard = (graph as FSMGraph).Blackboard;
      object initialized = null;
      blackboard.variables.TryGetValue("Init" + GetInstanceID(), out initialized);
      // If machine hasn't been initialized it is time to do so.
      if (initialized == null) {
        _tree.Init((graph as FSMGraph).Blackboard);
        blackboard.variables["Init" + GetInstanceID()] = true;
      }
    }

    public override void Exit() { }
  }
}