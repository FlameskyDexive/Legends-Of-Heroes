using UnityEngine;

namespace Planilo.FSM.Custom {

  /// <summary>A minimal debug example of a State Node</summary>
  [CreateNodeMenu("FSM/State/Debug")]
  class DebugState : FSMStateNode {

    /// <summary>Executes the state.</summary>
    public override void Run() {
      Debug.Log(name);
    }
  }
}