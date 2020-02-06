// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="Float2ControllerState.Transition"/>.
    /// </summary>
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Controller Transition/Float 2", order = Strings.AssetMenuOrder + 6)]
    public class Float2ControllerTransition : AnimancerTransition<Float2ControllerState.Transition> { }
}

