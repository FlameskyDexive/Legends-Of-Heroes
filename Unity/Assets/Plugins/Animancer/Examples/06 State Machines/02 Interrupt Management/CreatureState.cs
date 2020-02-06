// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.InterruptManagement
{
    /// <summary>
    /// A state for a <see cref="Creature"/> which plays an animation and uses a <see cref="Priority"/>
    /// enum to determine which other states can interrupt it.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Interrupt Management - Creature State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.InterruptManagement/CreatureState")]
    public sealed class CreatureState : StateBehaviour<CreatureState>
    {
        /************************************************************************************************************************/

        /// <summary>Levels of importance.</summary>
        public enum Priority
        {
            Low,// Could specify "Low = 0," if we want to be explicit.
            Medium,// Medium = 1,
            High,// High = 2,
        }

        /************************************************************************************************************************/

        [SerializeField] private Creature _Creature;
        [SerializeField] private AnimationClip _Animation;
        [SerializeField] private Priority _Priority;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            var state = _Creature.Animancer.Play(_Animation, 0.25f);
            if (!_Animation.isLooping)
                state.Events.OnEnd = _Creature.ForceIdleState;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Only allows a new state to be entered if it has equal or higher <see cref="Priority"/> to this state.
        /// </summary>
        public override bool CanExitState(CreatureState nextState)
        {
            return nextState._Priority >= _Priority;
        }

        /************************************************************************************************************************/
    }
}
