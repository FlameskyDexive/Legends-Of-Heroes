// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for Mixers in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;
using UnityEngine.Events;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Game Kit - Landing State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/LandingState")]
    public sealed class LandingState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private MixerState.Transition2D _SoftLanding;
        [SerializeField] private ClipState.Transition _HardLanding;
        [SerializeField] private float _HardLandingForwardSpeed = 5;
        [SerializeField] private float _HardLandingVerticalSpeed = -10;
        [SerializeField] private UnityEvent _PlayAudio;// See the Read Me.

        private bool _IsSoftLanding;

        /************************************************************************************************************************/

        private void Awake()
        {
            _SoftLanding.Events.Sequence.OnEnd =
                _HardLanding.Events.Sequence.OnEnd =
                () => Creature.CheckMotionState();
        }

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.CharacterController.isGrounded;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Performs either a hard or soft landing depending on the current speed (both horizontal and vertical).
        /// </summary>
        private void OnEnable()
        {
            Creature.ForwardSpeed = Creature.DesiredForwardSpeed;

            if (Creature.VerticalSpeed <= _HardLandingVerticalSpeed &&
                Creature.ForwardSpeed >= _HardLandingForwardSpeed)
            {
                _IsSoftLanding = false;
                Creature.Animancer.Play(_HardLanding);
            }
            else
            {
                _IsSoftLanding = true;
                Creature.Animancer.Play(_SoftLanding);
                _SoftLanding.State.Parameter = new Vector2(Creature.ForwardSpeed, Creature.VerticalSpeed);
            }

            // The landing sounds in the full 3D Game Kit have different variations based on the ground material, just
            // like the footstep sounds as discussed in the LocomotionState.

            _PlayAudio.Invoke();
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl { get { return _IsSoftLanding; } }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (!Creature.CharacterController.isGrounded &&
                Creature.StateMachine.TrySetState(Creature.Airborne))
                return;

            Creature.UpdateSpeedControl();

            if (_IsSoftLanding)
            {
                // Update the horizontal speed but keep the initial vertical speed from when you first landed.
                var parameter = _SoftLanding.State.Parameter;
                parameter.x = Creature.ForwardSpeed;
                _SoftLanding.State.Parameter = parameter;
            }
        }

        /************************************************************************************************************************/
    }
}
