using System.Collections;
using System.Collections.Generic;
using Planilo;
using UnityEngine;
using XNode;

/// <summary> A blackboard node for a variable of MonoBehaviour type.</summary>
[CreateNodeMenu("Blackboard/MonoBehaviour")]
public class BlackboardMonoBehaviour : BlackboardVariableNode<MonoBehaviour> { }