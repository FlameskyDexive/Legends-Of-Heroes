using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.FSM {

  /// <summary>Base node for all state nodes in a Finite State Machine.</summary>
  [CreateNodeMenu("")]
  [NodeTint("#2e4e6b")]
  public abstract class FSMStateNode : FSMNode {

    /// <summary>Get/Set whether this is the entry state.</summary>
    public bool IsEntry {
      get { return _isEntry; }
      set {
        if (value) {
          FSM.SetEntryState(this);
        } else {
          FSM.UnsetEntryState(this);
        }
        // Set flag value.
        _isEntry = value;
      }
    }

    /// <summary>Set this node as the entry state.</summary>
    [ContextMenu("Set as entry state")]
    public void SetAsRoot() {
      IsEntry = true;
    }

    /// <summary>Whether this is the entry state.</summary>
    [SerializeField][HideInInspector] bool _isEntry;
    /// <summary>List of entry connections.</summary>
    [SerializeField][HideInInspector] List<FSMConnection> entries = new List<FSMConnection>();
    /// <summary>List of exit connections.</summary>
    [SerializeField][HideInInspector] List<FSMConnection> exits = new List<FSMConnection>();

    /// <summary>Initialize state for execution.</summary>
    protected override void Init() {
      base.Init();
      // Init connections.
      foreach (FSMConnection connection in exits) {
        // Make sure connection is valid.
        if (connection == null) { continue; }
        // Init connection.
        connection.Init();
      }
    }

    /// <summary>Execute the state.</summary>
    public abstract void Run();

    /// <summary>Executed when entering the state.</summary>
    public virtual void Enter() { }

    /// <summary>Executed when exiting the state.</summary>
    public virtual void Exit() { }

    /// <summary>Return the state to which we need to transition.</summary>
    public FSMStateNode GetTransition() {
      FSMStateNode output = null;
      // Check all exits.
      foreach (FSMConnection connection in exits) {
        // Make sure connection is valid.
        if (connection == null) { continue; }
        // Get state from connection, if not null return it.
        output = connection.GetState();
        if (output != null) { return output; }
      }

      return null;
    }

    /// <summary>Amount of entry connections available.</summary>
    public int EntriesCount {
      get { return entries.Count; }
    }

    /// <summary>Get entry connection at position.</summary>
    public FSMConnection GetEntryConnection(int index) {
      return entries.Count > index ? entries[index] : null;
    }

    /// <summary>Add a new entry connection.</summary>
    public void AddEntryConnection(NodePort connection) {
      // Check port type.
      if (connection.ValueType != typeof(FSMConnection)) { return; }
      // Create new entry port.
      NodePort newport = AddInstanceInput(typeof(FSMConnection), Node.ConnectionType.Override);
      entries.Add(new FSMConnection(this, newport.fieldName));
      // Add connection.
      newport.Connect(connection);
    }

    /// <summary>Remove an entry connection at position.</summary>
    public void RemoveEntryConnection(int index) {
      // Make sure index is valid.
      if (entries.Count <= index) { return; }
      // Remove port and transition.
      RemoveInstancePort(entries[index].PortName);
      entries.RemoveAt(index);
    }

    /// <summary>Amount of exit connections available.</summary>
    public int ExitsCount {
      get { return exits.Count; }
    }

    /// <summary>Get exit connection at position.</summary>
    public FSMConnection GetExitConnection(int index) {
      return exits.Count > index ? exits[index] : null;
    }

    /// <summary>Add a new exit connection.</summary>
    public void AddExitConnection(NodePort connection) {
      // Check port type.
      if (connection.ValueType != typeof(FSMConnection)) { return; }
      // Create new exit port.
      NodePort newport = AddInstanceOutput(typeof(FSMConnection), Node.ConnectionType.Override);
      exits.Add(new FSMConnection(this, newport.fieldName));
      // Add connection.
      newport.Connect(connection);
    }

    /// <summary>Remove an exit connection at position.</summary>
    public void RemoveExitConnection(int index) {
      // Make sure index is valid.
      if (exits.Count <= index) { return; }
      // Remove port and transition.
      RemoveInstancePort(exits[index].PortName);
      exits.RemoveAt(index);
    }
  }
}