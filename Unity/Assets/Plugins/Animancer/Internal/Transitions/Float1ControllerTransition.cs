// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="Float1ControllerState.Transition"/>.
    /// </summary>
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Controller Transition/Float 1", order = Strings.AssetMenuOrder + 5)]
    public class Float1ControllerTransition : AnimancerTransition<Float1ControllerState.Transition> { }
}

