// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Brains
{
    /// <summary>
    /// A <see cref="CreatureBrain"/> which controls the creature using keyboard input.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Brains - Keyboard Brain")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Brains/KeyboardBrain")]
    public sealed class KeyboardBrain : CreatureBrain
    {
        /************************************************************************************************************************/

        [SerializeField] private CreatureState _Locomotion;

        /************************************************************************************************************************/

        private void Update()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input != Vector2.zero)
            {
                // Get the camera's forward and right vectors and flatten them onto the XZ plane.
                var camera = Camera.main.transform;

                var forward = camera.forward;
                forward.y = 0;
                forward.Normalize();

                var right = camera.right;
                right.y = 0;
                right.Normalize();

                // Build the movement vector by multiplying the input by those axes.
                MovementDirection =
                    right * input.x +
                    forward * input.y;

                // Determine if the player wants to run or not.
                IsRunning = Input.GetButton("Fire3");//Left Shift by default.

                // Enter the locomotion state if we aren't already in it.
                _Locomotion.TryEnterState();
            }
            else
            {
                // Clear the movement vector, though nothing should be using it while idle anyway.
                MovementDirection = Vector3.zero;

                // Enter the idle state if we aren't already in it.
                Creature.Idle.TryEnterState();
            }
        }

        /************************************************************************************************************************/
    }
}
