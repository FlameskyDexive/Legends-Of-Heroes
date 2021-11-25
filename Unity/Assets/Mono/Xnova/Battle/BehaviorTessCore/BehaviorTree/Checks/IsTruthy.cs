using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT.Custom {
  namespace BT {

	  /// <summary>A node for checking if a bool blackboard variable is true.</summary>
    [CreateNodeMenu("BT/Check/IsTruthy")]
    public class IsTruthy : BTTaskNode {

	  	/// <summary>The variable to check for null.</summary>
      [Input] public BlackboardBool _variable = null;

	  	/// <summary>Execute the check.</summary>
      protected override BTGraphResult InternalRun() {
        // Check if variable is null.
        bool variable = GetBlackboardValue<bool>("_variable", _variable);
        // If null return return state otherwise return null.
        return variable ? BTGraphResult.Success : BTGraphResult.Failure;
      }
    }
  }
}