using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planilo {

  /// <summary>Class for a AI graph blackboard.</summary>
  public class AIBlackboard : ScriptableObject {

    /// <summary>A serializable dictionary for the blackboard data.</summary>
    [System.Serializable]
    public class AIBlackboardDictionary : SerializableDictionary<string, object> { }

    /// <summary>A reference to the blackboard dictionary.</summary>
    [SerializeField] public AIBlackboardDictionary variables = new AIBlackboardDictionary();
  }
}