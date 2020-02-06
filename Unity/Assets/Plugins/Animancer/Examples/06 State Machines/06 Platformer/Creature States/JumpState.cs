// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A <see cref="CreatureState"/> that plays a jump animation and applies some upwards force.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Platformer - Jump State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Platformer/JumpState")]
    public class JumpState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimationClip _Animation;
        [SerializeField] private float _Height = 3;

        protected AnimancerState AnimancerState { get; private set; }

        /************************************************************************************************************************/

        public override float MovementSpeed
        {
            get { return Creature.Idle.MovementSpeed; }
        }

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.GroundDetector.IsGrounded;
        }

        /************************************************************************************************************************/

        protected virtual void OnEnable()
        {
            Creature.Rigidbody.velocity += new Vector2(0, CalculateJumpSpeed(_Height));

            AnimancerState = Creature.Animancer.Play(_Animation);
        }

        /************************************************************************************************************************/

        protected virtual void FixedUpdate()
        {
            // Wait until we are grounded and the animation has finished, then return to idle.
            if (Creature.GroundDetector.IsGrounded && AnimancerState.NormalizedTime > 1)
                Creature.Idle.ForceEnterState();
        }

        /************************************************************************************************************************/

        public float CalculateJumpSpeed(float height)
        {
            var gravity = Physics2D.gravity.y * Creature.Rigidbody.gravityScale;
            return Mathf.Sqrt(-2 * gravity * height);

            // This assumes gravity is negative (downwards), otherwise any force would give an infinite jump height.

            // If we wanted to support gravity in any direction, we could replace Physics2D.gravity.y with:
            // Vector2.Dot(Physics2D.gravity, Creature.transform.up)
            // We could have just done that here, but it is a bit less efficient and we do not need the complexity.
        }

        /************************************************************************************************************************/
    }
}
