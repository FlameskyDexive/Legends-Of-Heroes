// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="Float3ControllerState.Transition"/>.
    /// </summary>
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Controller Transition/Float 3", order = Strings.AssetMenuOrder + 7)]
    public class Float3ControllerTransition : AnimancerTransition<Float3ControllerState.Transition> { }
}

