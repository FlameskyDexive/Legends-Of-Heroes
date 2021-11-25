using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.FSM.Custom {

  /// <summary>A node for checking if a blackboard variable is null.</summary>
  [CreateNodeMenu("FSM/Check/IsNull")]
  public class IsNullCheck : FSMCheckNode {

    /// <summary>Execute the check.</summary>
    protected override bool InternalCheck() {
      // Check if variable is null.
      object variable = GetBlackboardValue<object>("_variable", _variable);
      bool isNull = variable == null;
      if (!isNull) {
        // Check if variable is of value type.
        System.Type type = variable.GetType();
        if (type.IsValueType) {
          isNull = variable == System.Activator.CreateInstance(type);
        }
      }
      // Return result.
      return isNull;
    }
  }
}