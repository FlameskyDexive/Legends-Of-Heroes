using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo {
  /// <summary>
  /// Base class for all AI graphs.
  /// </summary>
  public abstract class AIGraph : NodeGraph {

    /// <summary>Returns the current blackboard being executed.</summary>
    public AIBlackboard Blackboard {
      get { return _blackboard; }
    }
    protected AIBlackboard _blackboard;

    /// <summary>
    /// Get a value of type T from the blackboard by key name.
    /// </summary>
    /// <typeparam name="T">Type T needs to match a type or subtype of the saved value under key in the blackboard.</typeparam>
    /// <param name="key">The key of the desired value in the blackboard.</param>
    /// <returns>The value saved under key in the blackboard. <c>null</c> if type T is not a match or key doesn't exist.</returns>
    public T GetValue<T>(string key) {
      // Make sure blackboard is set.
      if (_blackboard == null) { return default(T); }
      // Check if value is present in the runner.
      object value;
      T result = default(T);
      if (_blackboard.variables.TryGetValue(key, out value)) {
        // Make sure the type is valid.
        result = value.GetType().IsAssignableFrom(typeof(T)) ? (T) value : default(T);
      }
      // Check if value is the correct type.
      return result;
    }

    /// <summary>
    /// Initialize the AIGraph in the given blackboard.
    /// </summary>
    /// <param name="blackboard"> The blackboard use to initialize the graph.</param>
    public abstract void Init(AIBlackboard blackboard);

    /// <summary>
    /// Run the AIGraph in the given blackboard.
    /// </summary>
    /// <param name="blackboard"> The blackboard use to execute the graph.</param>
    public abstract AIGraphResult Run(AIBlackboard blackboard);
  }
}