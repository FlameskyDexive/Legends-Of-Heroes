using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.FSM.Custom {

  /// <summary>A node for checking if a blackboard variable is null.</summary>
  [CreateNodeMenu("FSM/Check/IsTrue")]
  public class IsTrueCheck : FSMCheckNode {

    /// <summary>Initialize node for execution.</summary>
    protected override void Init() {
      _expectedType = typeof(BlackboardBool);
      base.Init();
    }

    /// <summary>Execute the check.</summary>
    protected override bool InternalCheck() {
      // Check if variable is null.
      bool variable = GetBlackboardValue<bool>("_variable", _variable);
      // Return result.
      return variable;
    }
  }
}