// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="ClipState.Transition"/>.
    /// </summary>
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Clip Transition", order = Strings.AssetMenuOrder + 0)]
    public class ClipTransition : AnimancerTransition<ClipState.Transition> { }
}

