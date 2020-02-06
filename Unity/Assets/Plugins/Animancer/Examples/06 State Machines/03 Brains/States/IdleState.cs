// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Brains
{
    /// <summary>
    /// A <see cref="CreatureState"/> which keeps the creature standing still.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Brains - Idle State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Brains/IdleState")]
    public sealed class IdleState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimationClip _Animation;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Play(_Animation, 0.25f);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Constantly clears the <see cref="Rigidbody.velocity"/> to ensure that the creature doesn't slide or get
        /// pushed around too easily.
        /// </summary>
        /// <remarks>
        /// This method is kept simple for the sake of demonstrating the animation system in this example.
        /// In a real game you would usually not set the velocity directly, but would instead use
        /// <see cref="Rigidbody.AddForce"/> to avoid interfering with collisions and other forces.
        /// </remarks>
        private void FixedUpdate()
        {
            Creature.Rigidbody.velocity = Vector3.zero;
        }

        /************************************************************************************************************************/
    }
}
