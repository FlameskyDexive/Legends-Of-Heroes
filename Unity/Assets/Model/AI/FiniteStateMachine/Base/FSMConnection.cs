using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Planilo.FSM {

  /// <summary>Class used for connecting nodes in the graph.</summary>
  [System.Serializable]
  public class FSMConnection {

    /// <summary>Node to which this connection belongs to.</summary>
    [SerializeField][HideInInspector] Node _node;
    /// <summary>Node to which this connects to.</summary>
    Node _connected;
    /// <summary>Node port by which the connection is done.</summary>
    [SerializeField][HideInInspector] string _portName;
    /// <summary>Is the connection empty?</summary>
    bool _isEmpty;
    /// <summary>Is the connection check node?</summary>
    bool _isCheck;
    /// <summary>Is the connection a state node?</summary>
    bool _isState;

    /// <summary>Get the port name.</summary>
    public string PortName {
      get { return _portName; }
    }

    /// <summary>Get whether it is connected.</summary>
    public bool Connected {
      get { return _connected; }
    }

    /// <summary>Class constructor.</summary>
    public FSMConnection(Node node, string portName) {
      // Cache node and port.
      _node = node;
      _portName = portName;
    }

    /// <summary>Initialize connection for execution.</summary>
    public void Init() {
      // Cache type of connected node.
      NodePort port = _node.GetOutputPort(_portName);
      _isEmpty = port == null || port.Connection == null;
      _connected = !_isEmpty ? port.Connection.node : null;
      _isCheck = !_isEmpty && _connected.GetType().IsSubclassOf(typeof(FSMCheckNode));
      _isState = !_isEmpty && _connected.GetType().IsSubclassOf(typeof(FSMStateNode));
    }

    /// <summary>Get the connection state node if any.</summary>
    public FSMStateNode GetState() {
      // Return null if not connected.
      if (_isEmpty) { return null; }
      // If connected is a check make it.
      if (_isCheck) {
        FSMCheckNode transition = _connected as FSMCheckNode;
        return transition.Check();
        // Return connected in case it is a state.
      } else if (_isState) {
        return _connected as FSMStateNode;
        // In unforeseen cases return null.
      } else {
        return null;
      }
    }
  }
}