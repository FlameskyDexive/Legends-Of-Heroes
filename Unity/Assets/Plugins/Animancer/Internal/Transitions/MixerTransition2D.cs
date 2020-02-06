// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="MixerState.Transition2D"/>.
    /// </summary>
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Mixer Transition/2D", order = Strings.AssetMenuOrder + 3)]
    public class MixerTransition2D : AnimancerTransition<MixerState.Transition2D> { }
}

