// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// Animates a simple character to be able to stand idle or walk forwards or backwards based on user input.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Locomotion - Idle And Walk")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Locomotion/IdleAndWalk")]
    public class IdleAndWalk : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer { get { return _Animancer; } }

        [SerializeField]
        private AnimationClip _Idle;
        public AnimationClip Idle { get { return _Idle; } }

        [SerializeField]
        private AnimationClip _Walk;
        public AnimationClip Walk { get { return _Walk; } }

        /************************************************************************************************************************/

        protected void Update()
        {
            // W or UpArrow = 1.
            // S or DownArrow = -1.
            // Otherwise 0.
            var movement = Input.GetAxisRaw("Vertical");
            if (movement != 0)
            {
                PlayMove();

                // Since we don't have animations for moving backwards, just use the input as their speed so that
                // moving backwards simply plays the animation backwards.
                _Animancer.States.Current.Speed = movement;

                // PlayMove could return the AnimancerState it plays, but using the CurrentState saves a bit of effort.
            }
            else
            {
                // If we aren't moving, return to idle.
                _Animancer.Play(_Idle, 0.25f);
            }
        }

        /************************************************************************************************************************/

        // We want to override this method in the IdleAndWalkAndRun script.
        protected virtual void PlayMove()
        {
            _Animancer.Play(_Walk, 0.25f);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If you add a second script derived from this type to the same object, it will instead change the type of
        /// the existing component, allowing you to easily swap between <see cref="IdleAndWalk"/> and
        /// <see cref="IdleAndWalkAndRun"/>.
        /// </summary>
        protected virtual void Reset()
        {
            AnimancerUtilities.IfMultiComponentThenChangeType(this);
        }

        /************************************************************************************************************************/
    }
}
