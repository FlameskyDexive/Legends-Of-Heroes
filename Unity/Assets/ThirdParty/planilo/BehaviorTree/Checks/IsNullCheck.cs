using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT.Custom {

  /// <summary>A node for checking if a blackboard variable is null.</summary>
  [CreateNodeMenu("BT/Check/IsNull")]
  public class IsNullCheck : BTTaskNode {

    /// <summary>The variable to check for null.</summary>
    [Input] public BlackboardBaseNode _variable = null;

    /// <summary>Execute the check.</summary>
    protected override BTGraphResult InternalRun() {
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
      // If null return return state otherwise return null.
      return isNull ? BTGraphResult.Success : BTGraphResult.Failure;
    }
  }
}