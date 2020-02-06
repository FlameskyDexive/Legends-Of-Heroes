// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// Keeps track of the health of an object.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Platformer - Health")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Platformer/Health")]
    [DefaultExecutionOrder(-5000)]// Initialise the CurrentHealth earlier than anything else will use it.
    public sealed class Health : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private int _MaxHealth;
        public int MaxHealth { get { return _MaxHealth; } }

        /************************************************************************************************************************/

        private int _CurrentHealth;
        public int CurrentHealth
        {
            get { return _CurrentHealth; }
            set
            {
                _CurrentHealth = Mathf.Clamp(value, 0, _MaxHealth);
                if (OnHealthChanged != null)
                    OnHealthChanged();
                else if (_CurrentHealth == 0)
                    Destroy(gameObject);
            }
        }

        public event Action OnHealthChanged;

        /************************************************************************************************************************/

        private void Awake()
        {
            CurrentHealth = _MaxHealth;
        }

        /************************************************************************************************************************/
    }
}
