// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for NormalizedEndTime in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Game Kit - Attack State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/AttackState")]
    public sealed class AttackState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private float _TurnSpeed = 400;
        [SerializeField] private UnityEvent _SetWeaponOwner;// See the Read Me.
        [SerializeField] private UnityEvent _OnStart;// See the Read Me.
        [SerializeField] private UnityEvent _OnEnd;// See the Read Me.
        [SerializeField] private ClipState.Transition[] _Animations;

        private int _AttackIndex = int.MaxValue;
        private ClipState.Transition _Attack;

        /************************************************************************************************************************/

        private void Awake()
        {
            _SetWeaponOwner.Invoke();
        }

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.CharacterController.isGrounded;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Start at the beginning of the sequence by default, but if the previous attack hasn't faded out yet then
        /// perform the next attack instead.
        /// </summary>
        private void OnEnable()
        {
            if (_AttackIndex >= _Animations.Length - 1 ||
                _Animations[_AttackIndex].State.Weight == 0)
            {
                _AttackIndex = 0;
            }
            else
            {
                _AttackIndex++;
            }

            _Attack = _Animations[_AttackIndex];
            Creature.Animancer.Play(_Attack);
            Creature.ForwardSpeed = 0;
            _OnStart.Invoke();
        }

        /************************************************************************************************************************/

        private void OnDisable()
        {
            _OnEnd.Invoke();
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl { get { return false; } }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (Creature.CheckMotionState())
                return;

            Creature.TurnTowards(Creature.Brain.Movement, _TurnSpeed);
        }

        /************************************************************************************************************************/

        public override bool CanExitState(CreatureState nextState)
        {
            // Use the End Event time to determine when this state is alowed to exit.

            // We cannot simply have this method return false and set the End Event to call Creature.CheckMotionState
            // because it uses TrySetState (instead of ForceSetState) which would be prevented by returning false here.

            // And we cannot have this method return true because that would allow other actions like jumping in the
            // middle of an attack.

            return _Attack.State.NormalizedTime >= _Attack.State.Events.NormalizedEndTime;
        }

        /************************************************************************************************************************/
    }
}
