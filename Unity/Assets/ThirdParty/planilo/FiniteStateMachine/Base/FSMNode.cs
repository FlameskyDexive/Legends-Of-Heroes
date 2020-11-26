using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.FSM {

  /// <summary>Base node for all nodes in a Finite State Machine.</summary>
  [CreateNodeMenu("")]
  public abstract class FSMNode : Node {

    /// <summary>Get the behavior tree.</summary>
    protected FSMGraph FSM {
      get { return graph as FSMGraph; }
    }

    /// <summary>Return port value.</summary>
    public override object GetValue(NodePort port) {
      return null; // Replace this
    }

    /// <summary>Get a value from a blackboard node connected to a port.</summary>
    /// <param name="portName">Port name to check for the blackboard variable.</param>
    /// <param name="node">Manually loaded blackboard variable node.</param>
    /// <param name="fallback">Fallback value, defaults to default type value.</param>
    /// <typeparam ref="T">Type of the value returned by the blackboard node.</param>
    public T GetBlackboardValue<T>(string portName, BlackboardBaseNode node, T fallback = default(T)) {
      // Check if we need to change the fallback from the value in the node.
      T value = node != null ? (T) node.GetValue() : fallback;
      // Check connection override.
      value = GetInputValue<T>(portName, value);
      return value;
    }

    /// <summary>Get value from input.</summary>
    public T GetInputValue<T>(string portName) {
      NodePort port = GetInputPort(portName);
      return (T) GetValue(port);
    }

    /// <summary>Add component to the current game object.</summary>
    /// <remark>Triggers a warning to remember designer to add them manually.</remark>
    protected T AddComponent<T>() where T : MonoBehaviour {
      // Get game object.
      GameObject go = FSM.GetValue<GameObject>("GameObject");
      if (go == null) {
        Debug.LogError("No game object running this finite state machine.");
        return null;
      }
      // Add component and save it in blackboard.
      T component = go.AddComponent<T>();
      Debug.LogWarning(string.Format("Adding component {0} on execution.", component.ToString()));
      return component;
    }

    /// <summary>Gets a component from the current game object.</summary>
    /// <remark>Triggers a warning when not present.</remark>
    protected T GetComponent<T>(string key) where T : MonoBehaviour {
      // Get Chooser.
      T component = FSM.GetValue<T>(key) as T;
      if (component == null) {
        component = AddComponent<T>();
      }
      // Return it.
      return component;
    }
  }
}