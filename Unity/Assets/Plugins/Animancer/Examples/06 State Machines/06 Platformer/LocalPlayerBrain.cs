// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A brain for creatures controlled by local input (keyboard and mouse).
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Platformer - Local Player Brain")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Platformer/LocalPlayerBrain")]
    public sealed class LocalPlayerBrain : CreatureBrain
    {
        /************************************************************************************************************************/

        [SerializeField] private CreatureState _Attack;
        [SerializeField] private AdvancedJumpState _Jump;

        /************************************************************************************************************************/

        private void Update()
        {
            if (Input.GetButtonUp("Fire1"))// Left Click by default.
                Creature.StateMachine.TrySetState(_Attack);

            if (Input.GetButtonDown("Jump"))// Space by default.
                Creature.StateMachine.TrySetState(_Jump);

            if (Input.GetButtonUp("Jump"))
                _Jump.IsHolding = false;

            // GetAxisRaw rather than GetAxis because we don't want any smoothing.
            MovementDirection = Input.GetAxisRaw("Horizontal");// A and D or Arrow Keys by default.

            IsRunning = Input.GetButton("Fire3");// Left Shift by default.
        }

        /************************************************************************************************************************/
    }
}
