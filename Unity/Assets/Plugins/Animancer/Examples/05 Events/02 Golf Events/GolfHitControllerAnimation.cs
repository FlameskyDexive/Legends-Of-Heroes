// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// An <see cref="GolfHitController"/> that uses Animation Events.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Golf Events - Animation")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimationEvents/GolfHitControllerAnimation")]
    public sealed class GolfHitControllerAnimation : GolfHitController
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Calls the base <see cref="GolfHitController.Awake"/> method and register
        /// <see cref="GolfHitController.EndSwing"/> to be called whenever the swing animation ends.
        /// <para></para>
        /// Normally Animancer could call the registered method at the End Time defined in the transition, but in this
        /// case the <see cref="AnimationClip"/> used with this script has an Animation Event with the Function Name
        /// "End", which will execute the registered method when that event time passes.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Swing.Events.Sequence.OnEnd = EndSwing;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls <see cref="GolfHitController.HitBall"/>. The <see cref="AnimationClip"/> used with this script has an
        /// Animation Event with the Function Name "Event", which will cause it to execute this method.
        /// <para></para>
        /// Normally you would just have the event use "HitBall" as its Function Name directly, but the same animation
        /// is also being used for <see cref="GolfHitControllerAnimationSimple"/> which relies on the Function Name
        /// being "Event".
        /// </summary>
        private void Event()
        {
            HitBall();
        }

        /************************************************************************************************************************/
    }
}
