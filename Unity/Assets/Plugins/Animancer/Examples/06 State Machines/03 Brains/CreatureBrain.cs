// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Brains
{
    /// <summary>
    /// Base class for controlling the actions of a <see cref="Brains.Creature"/>.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Brains - Creature Brain")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Brains/CreatureBrain")]
    public abstract class CreatureBrain : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;
        public Creature Creature
        {
            get { return _Creature; }
            set
            {
                // The More Brains example uses this to swap between brains at runtime.

                if (_Creature == value)
                    return;

                var oldCreature = _Creature;
                _Creature = value;

                // Make sure the old creature doesn't still reference this brain.
                if (oldCreature != null)
                    oldCreature.Brain = null;

                // Give the new creature a reference to this brain.
                // We also only want brains to be enabled when they actually have a creature to control.
                if (value != null)
                {
                    value.Brain = this;
                    enabled = true;
                }
                else
                {
                    enabled = false;
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>The direction this brain wants to move.</summary>
        public Vector3 MovementDirection { get; protected set; }

        /// <summary>Indicates whether this brain wants to run.</summary>
        public bool IsRunning { get; protected set; }

        /************************************************************************************************************************/
    }
}
