using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {

  /// <summary>Base class for all behavior tree nodes.</summary>
  [CreateNodeMenu("")]
  public abstract class BTNode : Node {
    /// <summary>Get the behavior tree.</summary>
    protected BTGraph BT {
      get { return graph as BTGraph; }
    }

    /// <summary>Get the parent port.</summary>
    public NodePort Parent {
      get { return GetInputPort("_parent"); }
    }
    /// <summary>Parent connection.</summary>
    [HideInInspector][Input] public BTConnection _parent;

    /// <summary>Prepare the node for execution.</summary>
    public void Prepare() {
      Init();
    }

    /// <summary>Return the value of a port.</summary>
    public override object GetValue(NodePort port) {
      return null;
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

    /// <summary>Execute the Behavior Tree node.</summary>
    public BTGraphResult Run() {
      // If running on editr save run time.
      if (Application.isEditor) {
        BT.Blackboard.variables["BTRunTime" + GetInstanceID()] = Time.time;
      }
      // Execute.
      return InternalRun();
    }

    /// <summary>Internally execute the Behavior Tree node.</summary>
    protected abstract BTGraphResult InternalRun();

    /// <summary>Add a component to the game object related to the current blackboard.</summary>
    protected T AddComponent<T>() where T : MonoBehaviour {
      // Get game object.
      GameObject go = BT.GetValue<GameObject>("GameObject");
      if (go == null) {
        Debug.LogError("No game object running this finite state machine.");
        return null;
      }
      // Add component and save it in blackboard.
      T component = go.AddComponent<T>();
      Debug.LogWarning(string.Format("Adding component {0} on execution.", component.ToString()));
      return component;
    }

    /// <summary>Get a component from the game object related to the current blackboard.</summary>
    protected T GetComponent<T>(string key) where T : MonoBehaviour {
      // Get Chooser.
      T component = BT.GetValue<T>(key) as T;
      if (component == null) {
        component = AddComponent<T>();
      }
      // Return it.
      return component;
    }
  }
}