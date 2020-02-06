// Animancer // Copyright 2020 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Animancer
{
    /// <summary>Plays a single <see cref="AnimationClip"/> on startup.</summary>
    [AddComponentMenu(Strings.MenuPrefix + "Solo Animation")]
    [HelpURL(Strings.APIDocumentationURL + "/SoloAnimation")]
    [DefaultExecutionOrder(-5000)]// Initialise before anything else tries to use this component.
    public sealed class SoloAnimation : SoloAnimationInternal { }
}
