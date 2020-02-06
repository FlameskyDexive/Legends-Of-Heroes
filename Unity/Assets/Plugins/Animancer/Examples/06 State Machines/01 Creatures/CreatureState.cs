// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Creatures
{
    /// <summary>
    /// A state for a <see cref="Creature"/> which simply plays an animation.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Creatures - Creature State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Creatures/CreatureState")]
    public sealed class CreatureState : StateBehaviour<CreatureState>
    {
        /************************************************************************************************************************/

        [SerializeField] private Creature _Creature;
        [SerializeField] private AnimationClip _Animation;

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the animation and if it is not looping it returns the <see cref="Creature"/> to Idle afterwards.
        /// </summary>
        private void OnEnable()
        {
            var state = _Creature.Animancer.Play(_Animation, 0.25f);
            if (!_Animation.isLooping)
                state.Events.OnEnd = _Creature.ForceIdleState;
        }

        /************************************************************************************************************************/
    }
}
