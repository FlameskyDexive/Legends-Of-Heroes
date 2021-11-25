using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planilo.BT {

  /// <summary>A builder for dynamic Behavior Trees based on a selector and multiple subtrees.</summary>
  /// <remarks>Inherits from <see cref="UnityEngine.MonoBehaviour"/></remarks>
  [RequireComponent(typeof(AIGraphRunner))]
  public class BTBuilder : MonoBehaviour {

    /// <summary>Reference to the AIGraphRunner on the game object.</summary>
    AIGraphRunner _runner;

    /// <summary>List of subtrees to be added to the selector in order</summary>
    [SerializeField] List<BTGraph> _subtrees;

    void Awake() {
      _runner = GetComponent<AIGraphRunner>();
    }

    void Start() {
      // Create an instance of graph.
      BTGraph graph = ScriptableObject.CreateInstance(typeof(BTGraph).ToString()) as BTGraph;
      graph.name = "Graph" + GetInstanceID();
      // Create root as selector.
      Selector root = graph.AddNode<Selector>();
      root.IsRoot = true;
      // Add all subtrees as children of root in order.
      foreach (BTGraph subtree in _subtrees) {
        var node = graph.AddNode<BTSubtreeNode>();
        node.Tree = subtree;
        root.AddChildConnection(node.Parent);
      }
      // Initialize graph of runner
      _runner.Graph = graph;
    }
  }
}