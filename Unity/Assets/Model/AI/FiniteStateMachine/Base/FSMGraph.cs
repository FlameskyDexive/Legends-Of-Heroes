using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.FSM {

  /// <summary>Class to represent a Finite State Machine as an AI graph.</summary>
  [CreateAssetMenu(fileName = "Machine", menuName = "FSM/Machine", order = 1)]
  public class FSMGraph : AIGraph {

    /// <summary>The entry state of the machine.</summary>
    [SerializeField] private FSMStateNode _entryState;

    /// <summary>Set a state node as the entry state.</summary>
    public void SetEntryState(FSMStateNode state) {
      // Unset previous root.
      if (_entryState != null) {
        _entryState.IsEntry = false;
      }
      // Set new root.
      _entryState = state;
    }

    /// <summary>Unset the entry state node if it matches the given state.</summary>
    public void UnsetEntryState(FSMStateNode state) {
      // Only unset when the state passed is the same as the current one.
      if (state == _entryState) {
        _entryState = null;
      }
    }

    /// <summary>Init the graph in the given blackboard.</summary>
    public override void Init(AIBlackboard blackboard) {
      // Save blackboard while executing.
      _blackboard = blackboard;
      _blackboard.variables["CurrentState" + GetInstanceID()] = _entryState;
      // Trigger enter on root.
      _entryState.Enter();
    }

    /// <summary>Execute the graph in the given blackboard.</summary>
    public override AIGraphResult Run(AIBlackboard blackboard) {
      // Save blackboard while executing.
      _blackboard = blackboard;
      // Get current state from blackboard.
      object state;
      if (!_blackboard.variables.TryGetValue("CurrentState" + GetInstanceID(), out state)) { return null; }
      // Execute state.
      FSMStateNode currentState = state as FSMStateNode;
      currentState.Run();
      // Get transition if any.
      FSMStateNode newState = currentState.GetTransition();

      if (newState != null) {
        currentState.Exit();
        _blackboard.variables["CurrentState" + GetInstanceID()] = newState;
        newState.Enter();
      }

      // Nullify references to the blackboard.
      _blackboard = null;
      return null;
    }
  }
}