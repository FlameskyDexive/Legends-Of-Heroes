// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// Inherits from <see cref="IdleAndWalk"/> and adds the ability to run.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Locomotion - Idle And Walk And Run")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Locomotion/IdleAndWalkAndRun")]
    public class IdleAndWalkAndRun : IdleAndWalk
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimationClip _Run;

        /************************************************************************************************************************/

        protected override void PlayMove()
        {
            // We will play either the Walk or Run animation.

            // We need to know which animation we are trying to play and which is the other one.
            AnimationClip playAnimation, otherAnimation;

            if (Input.GetButton("Fire3"))// Left Shift by default.
            {
                playAnimation = _Run;
                otherAnimation = Walk;
            }
            else
            {
                playAnimation = Walk;
                otherAnimation = _Run;
            }

            // Play the one we want.
            var playState = Animancer.Play(playAnimation, 0.25f);

            // If the other one is still fading out, align their NormalizedTime to ensure they stay at the same
            // relative progress through their walk cycle.
            var otherState = Animancer.States[otherAnimation];
            if (otherState != null && otherState.IsPlaying)
                playState.NormalizedTime = otherState.NormalizedTime;
        }

        /************************************************************************************************************************/
    }
}
