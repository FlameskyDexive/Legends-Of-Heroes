// Animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A component which uses Animation Events with the Function Name "Event" to trigger a callback.
    /// <para></para>
    /// This component must always be attached to the same <see cref="GameObject"/> as the <see cref="Animator"/> in
    /// order to receive Animation Events from it.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Simple Event Receiver")]
    [HelpURL(Strings.APIDocumentationURL + "/SimpleEventReceiver")]
    public class SimpleEventReceiver : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>A callback for Animation Events with the Function Name "Event".</summary>
        /// <remarks>
        /// This field must be public instead of being wrapped in a property since <see cref="AnimationEventReceiver"/>
        /// is a struct. Otherwise another class calling <c>receiver.onEvent.Set(...)</c> would actually get a copy of
        /// the <c>onEvent</c>, set the desired values on that copy, and then immediately discard the copy without
        /// actually modifying the underlying field.
        /// </remarks>
        public AnimationEventReceiver onEvent;

        /// <summary>Called by Animation Events with the Function Name "Event".</summary>
        private void Event(AnimationEvent animationEvent)
        {
            onEvent.SetFunctionName("Event");// The name of this method.
            onEvent.HandleEvent(animationEvent);
        }

        /************************************************************************************************************************/
    }
}
