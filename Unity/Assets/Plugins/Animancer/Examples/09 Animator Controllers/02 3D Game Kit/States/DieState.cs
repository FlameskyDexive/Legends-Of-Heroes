// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Game Kit - Die State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/DieState")]
    public sealed class DieState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private ClipState.Transition _Animation;
        [SerializeField] private UnityEvent _OnEnterState;// See the Read Me.
        [SerializeField] private UnityEvent _OnExitState;// See the Read Me.

        /************************************************************************************************************************/

        private void Awake()
        {
            // Respawn immediately when the animation ends.
            _Animation.Events.Sequence.OnEnd = Creature.Respawn.ForceEnterState;
        }

        /************************************************************************************************************************/

        public void OnDeath()
        {
            Creature.StateMachine.ForceSetState(this);
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Play(_Animation);
            Creature.ForwardSpeed = 0;
            _OnEnterState.Invoke();
        }

        /************************************************************************************************************************/

        private void OnDisable()
        {
            _OnExitState.Invoke();
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl { get { return false; } }

        /************************************************************************************************************************/

        public override bool CanExitState(CreatureState nextState)
        {
            return false;
        }

        /************************************************************************************************************************/
    }
}
