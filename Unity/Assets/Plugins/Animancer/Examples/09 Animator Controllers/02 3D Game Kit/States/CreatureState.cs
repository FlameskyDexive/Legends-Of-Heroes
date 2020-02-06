// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// Base class for the various states a <see cref="Brains.Creature"/> can be in and actions they can perform.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Game Kit - Creature State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/CreatureState")]
    public abstract class CreatureState : StateBehaviour<CreatureState>,
        IOwnedState<CreatureState>
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;

        /// <summary>The <see cref="Brains.Creature"/> that owns this state.</summary>
        public Creature Creature
        {
            get { return _Creature; }
        }

#if UNITY_EDITOR
        protected void Reset()
        {
            _Creature = Editor.AnimancerEditorUtilities.GetComponentInHierarchy<Creature>(gameObject);
        }
#endif

        /************************************************************************************************************************/

        public StateMachine<CreatureState> OwnerStateMachine { get { return _Creature.StateMachine; } }

        /************************************************************************************************************************/

        /// <summary>
        /// Jumping enters the <see cref="AirborneState"/>, but <see cref="CharacterController.isGrounded"/> doesn't
        /// become false until after the first update, so we want to make sure the <see cref="Creature"/> won't stick
        /// to the ground during that update.
        /// </summary>
        public virtual bool StickToGround { get { return true; } }

        /// <summary>
        /// Some states (such as <see cref="AirborneState"/>) will want to apply their own source of root motion, but
        /// most will just use the root motion from the animations.
        /// </summary>
        public virtual Vector3 RootMotion { get { return _Creature.Animancer.Animator.deltaPosition; } }

        /// <summary>
        /// Indicates whether the root motion applied each frame while this state is active should be constrained to
        /// only move in the specified <see cref="CreatureBrain.Movement"/>. Otherwise the root motion can
        /// move the <see cref="Creature"/> in any direction. Default is true.
        /// </summary>
        public virtual bool FullMovementControl { get { return true; } }

        /************************************************************************************************************************/
    }
}
