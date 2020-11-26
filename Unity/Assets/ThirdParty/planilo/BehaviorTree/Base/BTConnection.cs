using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.BT {
  /// <summary>Class used for connecting nodes in the graph.</summary>
  [System.Serializable]
  public class BTConnection {
    /// <summary>Node to which this connection belongs to.</summary>
    [SerializeField][HideInInspector] Node _node;
    /// <summary>Node to which this connects to.</summary>
    Node _connected;
    /// <summary>Port name through which this connects.</summary>
    [SerializeField][HideInInspector] string _portName;
    /// <summary>Is the port connected?</summary>
    bool _isEmpty;

    /// <summary>Returns the port name.</summary>
    public string PortName {
      get { return _portName; }
    }

    /// <summary>Returns the connected node.</summary>
    public bool Connected {
      get { return _connected; }
    }

    /// <summary>Creates a new connection for a given node and port.</summary>
    public BTConnection(Node node, string portName) {
      // Cache node and port.
      _node = node;
      _portName = portName;
      // Init.
      Init();
    }

    /// <summary>Initializes the connection caching required data for execution.</summary>
    public void Init() {
      // Cache type of connected node.
      NodePort port = _node.GetOutputPort(_portName);
      _isEmpty = port == null || port.Connection == null;
      _connected = !_isEmpty ? port.Connection.node : null;
    }

    /// <summary>Runs the child node connected through this connection.</summary>
    public BTGraphResult Run() {
      // Return null if not connected.
      if (_isEmpty) { return BTGraphResult.Failure; }
      return (_connected as BTNode).Run();
    }
  }
}