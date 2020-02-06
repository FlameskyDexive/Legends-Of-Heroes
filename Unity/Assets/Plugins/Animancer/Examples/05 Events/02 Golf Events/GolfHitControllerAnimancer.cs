// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// An <see cref="GolfHitController"/> that uses Animancer Events configured entirely in the Inspector.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Golf Events - Animancer")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimationEvents/GolfHitControllerAnimancer")]
    public sealed class GolfHitControllerAnimancer : GolfHitController
    {
        /************************************************************************************************************************/

        // Nothing here.
        // This script is no different from the base GolfHitController.
        // It assumes the events are already fully configured in the Inspector.

        /************************************************************************************************************************/
    }
}
