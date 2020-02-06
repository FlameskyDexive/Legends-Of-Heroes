// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using System;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// A centralised group of references to the common parts of a creature and a state machine for their actions.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Game Kit - Creature")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/Creature")]
    [DefaultExecutionOrder(-5000)]// Initialise the State Machine early.
    public sealed class Creature : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer { get { return _Animancer; } }

        [SerializeField]
        private CharacterController _CharacterController;
        public CharacterController CharacterController { get { return _CharacterController; } }

        [SerializeField]
        private CreatureBrain _Brain;
        public CreatureBrain Brain
        {
            get { return _Brain; }
            set
            {
                if (_Brain == value)
                    return;

                var oldBrain = _Brain;
                _Brain = value;

                // Make sure the old brain doesn't still reference this creature.
                if (oldBrain != null)
                    oldBrain.Creature = null;

                // Give the new brain a reference to this creature.
                if (value != null)
                    value.Creature = this;
            }
        }

        /// <summary>Inspector toggle so you can easily compare raw root motion with controlled motion.</summary>
        [SerializeField]
        private bool _FullMovementControl = true;

        [SerializeField]
        private CreatureStats _Stats;
        public CreatureStats Stats { get { return _Stats; } }

        /************************************************************************************************************************/

        [Header("States")]
        [SerializeField]
        private CreatureState _Respawn;
        public CreatureState Respawn { get { return _Respawn; } }

        [SerializeField]
        private CreatureState _Idle;
        public CreatureState Idle { get { return _Idle; } }

        [SerializeField]
        private CreatureState _Locomotion;
        public CreatureState Locomotion { get { return _Locomotion; } }

        [SerializeField]
        private AirborneState _Airborne;
        public AirborneState Airborne { get { return _Airborne; } }

        /************************************************************************************************************************/

        public StateMachine<CreatureState> StateMachine { get; private set; }

        /// <summary>
        /// Forces the <see cref="StateMachine"/> to return to the <see cref="Idle"/> state.
        /// </summary>
        public Action ForceEnterIdleState { get; private set; }

        public float ForwardSpeed { get; set; }
        public float DesiredForwardSpeed { get; set; }
        public float VerticalSpeed { get; set; }

        public Material GroundMaterial { get; private set; }

        /************************************************************************************************************************/

        private void Awake()
        {
            // Note that this class has a [DefaultExecutionOrder] attribute to ensure that this method runs before any
            // other components that might want to access it.

            ForceEnterIdleState = () => StateMachine.ForceSetState(_Idle);

            StateMachine = new StateMachine<CreatureState>(_Respawn);
        }

        /************************************************************************************************************************/
        #region Motion
        /************************************************************************************************************************/

        /// <summary>
        /// Check if this <see cref="Creature"/> should enter the Idle, Locomotion, or Airborne states depending on
        /// whether it is grounded and the movement input from the <see cref="Brain"/>.
        /// </summary>
        /// <remarks>
        /// We could add some null checks to this method to support creatures that don't have all the standard states,
        /// such as a creature that can't move or a flying creature that never lands.
        /// </remarks>
        public bool CheckMotionState()
        {
            CreatureState state;
            if (_CharacterController.isGrounded)
            {
                state = _Brain.Movement == Vector3.zero ? _Idle : _Locomotion;
            }
            else
            {
                state = _Airborne;
            }

            return
                state != StateMachine.CurrentState &&
                StateMachine.TryResetState(state);
        }

        /************************************************************************************************************************/

        public void UpdateSpeedControl()
        {
            var movement = _Brain.Movement;
            movement = Vector3.ClampMagnitude(movement, 1);

            DesiredForwardSpeed = movement.magnitude * _Stats.MaxSpeed;

            var deltaSpeed = movement != Vector3.zero ? _Stats.Acceleration : _Stats.Deceleration;
            ForwardSpeed = Mathf.MoveTowards(ForwardSpeed, DesiredForwardSpeed, deltaSpeed * Time.deltaTime);
        }

        /************************************************************************************************************************/

        public float CurrentTurnSpeed
        {
            get
            {
                return Mathf.Lerp(
                    _Stats.MaxTurnSpeed,
                    _Stats.MinTurnSpeed,
                    ForwardSpeed / DesiredForwardSpeed);
            }
        }

        /************************************************************************************************************************/

        public bool GetTurnAngles(Vector3 direction, out float currentAngle, out float targetAngle)
        {
            if (direction == Vector3.zero)
            {
                currentAngle = float.NaN;
                targetAngle = float.NaN;
                return false;
            }

            var transform = this.transform;
            currentAngle = transform.eulerAngles.y;
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            return true;
        }

        /************************************************************************************************************************/

        public void TurnTowards(float currentAngle, float targetAngle, float speed)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, speed * Time.deltaTime);

            transform.eulerAngles = new Vector3(0, currentAngle, 0);
        }

        public void TurnTowards(Vector3 direction, float speed)
        {
            float currentAngle, targetAngle;
            if (GetTurnAngles(direction, out currentAngle, out targetAngle))
                TurnTowards(currentAngle, targetAngle, speed);
        }

        /************************************************************************************************************************/

        public void OnAnimatorMove()
        {
            var movement = GetRootMotion();
            CheckGround(ref movement);
            UpdateGravity(ref movement);
            _CharacterController.Move(movement);

            transform.rotation *= _Animancer.Animator.deltaRotation;
        }

        /************************************************************************************************************************/

        private Vector3 GetRootMotion()
        {
            var movement = StateMachine.CurrentState.RootMotion;

            // If the current state wants full movement control.
            if (_FullMovementControl && StateMachine.CurrentState.FullMovementControl)
            {
                // And we are actually trying to move.
                var direction = _Brain.Movement;
                direction.y = 0;
                if (direction == Vector3.zero)
                {
                    movement = Vector3.zero;
                }
                else
                {
                    // Then calculate the attempted movement in that direction and use only that.
                    var magnitude = Vector3.Dot(direction.normalized, movement);
                    movement = direction * magnitude;
                }
            }

            return movement;
        }

        /************************************************************************************************************************/

        private void CheckGround(ref Vector3 movement)
        {
            if (!CharacterController.isGrounded)
                return;

            const float GroundedRayDistance = 1f;

            RaycastHit hit;
            var ray = new Ray(transform.position + Vector3.up * GroundedRayDistance * 0.5f, -Vector3.up);
            if (Physics.Raycast(ray, out hit, GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                // Rotate the movement to lie along the ground vector.
                movement = Vector3.ProjectOnPlane(movement, hit.normal);

                // Store the current walking surface so the correct audio is played.
                var groundRenderer = hit.collider.GetComponentInChildren<Renderer>();
                GroundMaterial = groundRenderer ? groundRenderer.sharedMaterial : null;
            }
            else
            {
                GroundMaterial = null;
            }
        }

        /************************************************************************************************************************/

        private void UpdateGravity(ref Vector3 movement)
        {
            if (CharacterController.isGrounded && StateMachine.CurrentState.StickToGround)
                VerticalSpeed = -_Stats.Gravity * _Stats.StickingGravityProportion;
            else
                VerticalSpeed -= _Stats.Gravity * Time.deltaTime;

            movement.y += VerticalSpeed * Time.deltaTime;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Inspector Gadgets Pro calls this method after drawing the regular Inspector GUI, allowing this script to
        /// display its current state in Play Mode.
        /// </summary>
        /// <remarks>
        /// Inspector Gadgets Pro allows you to easily customise the Inspector without writing a full custom Inspector
        /// class by simply adding a method with this name. Without Inspector Gadgets, this method will do nothing.
        /// It can be purchased from https://kybernetik.com.au/inspector-gadgets/pro
        /// </remarks>
        private void AfterInspectorGUI()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                GUI.enabled = false;
                UnityEditor.EditorGUILayout.ObjectField("Current State", StateMachine.CurrentState, typeof(CreatureState), true);
                GUI.enabled = true;

                VerticalSpeed = UnityEditor.EditorGUILayout.FloatField("Vertical Speed", VerticalSpeed);
            }
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
