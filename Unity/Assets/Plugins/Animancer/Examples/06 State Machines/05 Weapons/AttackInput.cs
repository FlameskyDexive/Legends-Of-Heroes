// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.Examples.StateMachines.Brains;
using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Weapons
{
    /// <summary>
    /// Causes a <see cref="Creature"/> to attack in response to player input.
    /// <para></para>
    /// Normally this would be part of the <see cref="CreatureBrain"/>, but since we want to demonstrate both
    /// <see cref="KeyboardBrain"/> and <see cref="MouseBrain"/> in this example we have implemented it separately.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Weapons - Attack Input")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Weapons/AttackInput")]
    public sealed class AttackInput : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private CreatureState _Attack;
        [SerializeField] private float _AttackInputTimeOut = 0.5f;

        private StateMachine<CreatureState>.InputBuffer _InputBuffer;

        /************************************************************************************************************************/

        private void Awake()
        {
            _InputBuffer = new StateMachine<CreatureState>.InputBuffer(_Attack.Creature.StateMachine);
        }

        /************************************************************************************************************************/

        private void Update()
        {
            if (Input.GetButtonDown("Fire2"))// Right Click by default.
            {
                _InputBuffer.TrySetState(_Attack, _AttackInputTimeOut);
            }
            else
            {
                _InputBuffer.Update();
            }
        }

        /************************************************************************************************************************/
    }
}
