// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for MixerStates in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// An example of how you can use a <see cref="LinearMixerState"/> to mix a set of animations based on a
    /// <see cref="Speed"/> parameter.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Locomotion - Linear Mixer Locomotion")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Locomotion/LinearMixerLocomotion")]
    public sealed class LinearMixerLocomotion : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private LinearMixerTransition _Mixer;

        private LinearMixerState _State;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Animancer.Play(_Mixer);
            _State = _Mixer.Transition.State;
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
