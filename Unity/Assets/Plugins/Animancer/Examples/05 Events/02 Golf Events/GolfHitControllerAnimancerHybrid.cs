// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for Animancer Events in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// An <see cref="GolfHitController"/> that uses an Animancer Event which has its time set in the Inspector but its
    /// callback left blank so that it can be assigned by code (a "hybrid" between Inspector and Code based systems).
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Golf Events - Animancer Hybrid")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimationEvents/GolfHitControllerAnimancerHybrid")]
    public sealed class GolfHitControllerAnimancerHybrid : GolfHitController
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Calls the base <see cref="GolfHitController.Awake"/> method and register
        /// <see cref="GolfHitController.EndSwing"/> to be called whenever the swing animation ends.
        /// <para></para>
        /// The <see cref="GolfHitController._Swing"/> transition has its End Time set so that it will execute the
        /// registered method at some point during the animation, but its End Callback was left blank so it can be
        /// assigned here.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(_Swing.Events.Sequence.Count == 1, "Expected one event for hitting the ball", this);
            _Swing.Events.Sequence.Set(0, HitBall);

            // If we did not create the event in the Inspector, we could add it here:
            //_Swing.Events.Sequence.Add(new AnimancerEvent(0.375f, OnHitBall));

            _Swing.Events.Sequence.OnEnd = EndSwing;
        }

        /************************************************************************************************************************/
    }
}
