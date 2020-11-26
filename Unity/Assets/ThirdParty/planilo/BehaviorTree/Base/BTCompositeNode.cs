using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Planilo.BT {

  /// <summary>Base class for composite nodes of the Behavior Tree.</summary>
  /// <remarks>Inherits from <see cref="Planilo.BT.BTBranchNode"/>.</remarks>
  [CreateNodeMenu("")]
  [NodeTint("#2e4e6b")]
  public abstract class BTCompositeNode : BTBranchNode {

    /// <summary>List of children connections for the node.</summary>
    [SerializeField][HideInInspector] protected List<BTConnection> _children = new List<BTConnection>();

    // [Output(dynamicPortList = true)] public BTConnection[] _list = new BTConnection[0];

    /// <summary>Initializer called on creation and preparation.</summary>
    protected override void Init() {
      base.Init();
      // Init connections.
      foreach (BTConnection connection in _children) {
        // Make sure connection is valid.
        if (connection == null) { continue; }
        // Init connection.
        connection.Init();
      }
    }

    /// <summary>Count of child nodes connected.</summary>
    public int ChildCount {
      get { return _children.Count; }
    }

    /// <summary>Gets the children connection by index.</summary>
    /// <returns>The specified BTConnection.</returns>
    /// <param name="index">The index to request.</param>
    public BTConnection GetChildConnection(int index) {
      return _children.Count > index ? _children[index] : null;
    }

    /// <summary>Adds a new children connection.</summary>
    /// <param name="connection">The NodePort to connect the new child connection to.</param>
    public virtual void AddChildConnection(NodePort connection) {
      // Check port type.
      if (connection.ValueType != typeof(BTConnection)) { return; }
      // Create new exit port.
      NodePort newport = AddInstanceOutput(typeof(BTConnection), Node.ConnectionType.Override);
      _children.Add(new BTConnection(this, newport.fieldName));
      // Add connection.
      newport.Connect(connection);
    }

    /// <summary>Removes a children connection at index position.</summary>
    /// <param name="index">The position of the child to remove.</param>
    public virtual void RemoveChildConnection(int index) {
      // Make sure index is valid.
      if (_children.Count <= index) { return; }
      // Remove port and transition.
      RemoveInstancePort(_children[index].PortName);
      _children.RemoveAt(index);
    }

    /// <summary>Execute the Behavior Tree.</summary>
    protected override BTGraphResult InternalRun() {
      // Init loop.
      int[] indices;
      object value;
      if (BT.Blackboard.variables.TryGetValue("CompositeIndices" + GetInstanceID(), out value)) {
        indices = (int[]) value;
      } else {
        indices = PrepareChildren();
      }
      // Traverse all children running them until break point.
      BTGraphResult result = BTGraphResult.Failure;
      for (int i = 0; i < indices.Length; i++) {
        // Run target child.
        BTConnection connection = _children[indices[i]];
        result = connection.Run();
        // Break if result is running.
        if (result.IsRunning) {
          indices = indices.Where((a, index) => index >= i).ToArray();
          BT.Blackboard.variables["CompositeIndices" + GetInstanceID()] = indices;
          break;
        }
        // Check if we need to break loop.
        if (ShouldBreakLoop(result)) { break; }
      }

      // Reset running child index if not still running.
      if (!result.IsRunning) {
        BT.Blackboard.variables.Remove("CompositeIndices" + GetInstanceID());
      }

      return result;
    }

    /// <summary>Prepare the order in which to execute the children nodes.</summary>
    /// <returns>Array of ints containing the positions of the children nodes in the desired order.</returns>
    protected virtual int[] PrepareChildren() {
      int[] indices = new int[_children.Count];
      for (var i = 0; i < _children.Count; i++) {
        indices[i] = i;
      }
      return indices;
    }

    /// <summary>Whether the last result should break exexcution or not.</summary>
    /// <param name="result">The result of the last executed child.</param>
    protected abstract bool ShouldBreakLoop(BTGraphResult result);
  }
}