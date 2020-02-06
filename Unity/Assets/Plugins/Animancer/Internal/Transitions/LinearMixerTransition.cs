// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="LinearMixerState.Transition"/>.
    /// </summary>
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Mixer Transition/Linear", order = Strings.AssetMenuOrder + 2)]
    public class LinearMixerTransition : AnimancerTransition<LinearMixerState.Transition> { }
}

