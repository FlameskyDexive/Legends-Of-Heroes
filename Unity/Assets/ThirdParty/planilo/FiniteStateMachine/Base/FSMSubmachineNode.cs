using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planilo.FSM {

  /// <summary>Finite State Machine node that will execute a sub machine.</summary>
  [CreateNodeMenu("FSM/State/Submachine")]
  public class FSMSubmachineNode : FSMStateNode {

    /// <summary>A reference to the sub machine to be executed.</summary>
    [SerializeField] FSMGraph _machine;

    /// <summary>Execute state running submachine.</summary>
    public override void Run() {
      _machine.Run((graph as FSMGraph).Blackboard);
    }

    /// <summary>Executed on entering the state.</summary>
    public override void Enter() {
      // Check if machine has been initialized.
      AIBlackboard blackboard = (graph as FSMGraph).Blackboard;
      object initialized = null;
      blackboard.variables.TryGetValue("Init" + GetInstanceID(), out initialized);
      // If machine hasn't been initialized it is time to do so.
      if (initialized == null) {
        _machine.Init((graph as FSMGraph).Blackboard);
        blackboard.variables["Init" + GetInstanceID()] = true;
      }
    }

    /// <summary>Executed on exiting the state.</summary>
    public override void Exit() { }
  }
}