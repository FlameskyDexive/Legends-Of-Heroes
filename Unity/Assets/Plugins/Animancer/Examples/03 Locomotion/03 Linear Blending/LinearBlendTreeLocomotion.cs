// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for ControllerStates in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// An example of how you can wrap a <see cref="RuntimeAnimatorController"/> containing a single blend tree in a
    /// <see cref="Float1ControllerState"/> to easily control its parameter.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Locomotion - Linear Blend Tree Locomotion")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Locomotion/LinearBlendTreeLocomotion")]
    public sealed class LinearBlendTreeLocomotion : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private Float1ControllerTransition _Controller;

        private Float1ControllerState _State;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            // Since Float1ControllerTransition is a Transition Asset which can be shared among multiple objects, we
            // cannot simply access the _Controller.Transition.State whenever we want because it will only hold the
            // most recently played state (which will only be correct for one instance but not the others).

            // So instead, we grab the state right after playing it.

            _Animancer.Play(_Controller);
            _State = _Controller.Transition.State;

            // The state returned by the Play method would do the same thing, but it only returns a base AnimancerState
            // and we need a Float1ControllerState to access its Parameter property below so we would need to cast it:
            // _State = (Float1ControllerState)_Animancer.Play(_Controller);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Set by a <see cref="UnityEngine.Events.UnityEvent"/> on a <see cref="UnityEngine.UI.Slider"/>.
        /// </summary>
        public float Speed
        {
            get { return _State.Parameter; }
            set { _State.Parameter = value; }
        }

        /************************************************************************************************************************/
    }
}
