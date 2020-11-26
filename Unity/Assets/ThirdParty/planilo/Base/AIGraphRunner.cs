using UnityEngine;

namespace Planilo {
  /// <summary>Class for executing AI Graph attached to a Game Object. Inherits from <c>MonoBehaviour</c>.</summary>
  public class AIGraphRunner : MonoBehaviour {

    /// <summary>Delegate used for OnGraphInit event.abstract</summary>
    public delegate void GraphInit();
    /// <summary>Event executed when a new graph is initialized.</summary>
    public event GraphInit OnGraphInit;

    /// <summary>Blackboard containing the runner data.</summary>
    protected AIBlackboard _blackboard;

    /// <summary>Sets and initializes the graph to be run.</summary>
    public AIGraph Graph {
      set {
        _graph = value;
        // Do not continue if value is null.
        if (value == null) {
          _blackboard = null;
          return;
        }
        // Reset blackboard.
        InitBlackboard();
      }
    }

    /// <summary>Internal reference to the graph to be executed.</summary>
    [SerializeField] protected AIGraph _graph;

    /// <summary>Whether the runner has an initialized graph to be run.</summary>
    public bool Initialized {
      get { return _blackboard != null; }
    }

    /// <summary>Inits the blackboard for the current graph.</summary>
    protected void InitBlackboard() {
      // Set up blackboard.
      _blackboard = ScriptableObject.CreateInstance(typeof(AIBlackboard)) as AIBlackboard;
      SetInBlackboard("GameObject", gameObject);
      // Init tree.
      _graph.Init(_blackboard);
      // Execute on tree init event.
      if (OnGraphInit != null) { OnGraphInit(); }
    }

    /// <summary>Set a (key, value) pair in the graph's blackboard.</summary>
    /// <param key="key">Key under which the value will be saved.</param>
    /// <param key="value">Value of any type to be saved in the blackboard.</param>
    public void SetInBlackboard(string key, object value) {
      // Make sure blackboard is created
      if (_blackboard == null) { return; }
      // Set key.
      _blackboard.variables[key] = value;
    }

    /// <summary>Unset the value saved under a key in the graph's blackboard.</summary>
    /// <param key="key">Key for the value to be removed.</param>
    public void UnsetInBlackboard(string key) {
      // Make sure blackboard is created
      if (_blackboard == null) { return; }
      // Remove key.
      _blackboard.variables.Remove(key);
    }

    /// <summary>Return the value saved under a key in the graph's blackboard.</summary>
    /// <param key="key">Key for the value to be fetched.</param>
    public object GetFromBlackboard(string key) {
      // Make sure blackboard is created
      if (_blackboard == null) { return null; }
      // Return value if it exist.
      object value = null;
      _blackboard.variables.TryGetValue(key, out value);
      return value;
    }

    void Start() {
      // Make sure there is a machine to run.
      if (_graph != null) {
        Graph = _graph;
      }
    }

    void Update() {
      // Make sure we are good to go.
      if (!Initialized) { return; }
      // Execute machine.
      _graph.Run(_blackboard);
    }
  }
}