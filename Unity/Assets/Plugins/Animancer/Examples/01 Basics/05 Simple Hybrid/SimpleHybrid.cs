// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.Basics
{
    /// <summary>
    /// Demonstrates how to use a <see cref="HybridAnimancerComponent"/> to play animations in a
    /// <see cref="RuntimeAnimatorController"/> and separate <see cref="AnimationClip"/>s on the same character.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Basics - Simple Hybrid")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Basics/SimpleHybrid")]
    public sealed class SimpleHybrid : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private HybridAnimancerComponent _Animancer;
        [SerializeField] private AnimationClip _GolfSwing;
        [SerializeField] private AnimationClip _TennisForehand;

        /************************************************************************************************************************/

        // Called by a UI Button.
        public void SetIsWalking(bool value)
        {
            _Animancer.SetBool("IsWalking", value);
        }

        // Called by a UI Button.
        public void PlayRun()
        {
            _Animancer.Play("Run");
        }

        // Called by a UI Button.
        public void PlayTennisForehand()
        {
            _Animancer.Play(_TennisForehand);
        }

        // Called by a UI Button.
        public void PlayGolfSwing()
        {
            _Animancer.Play(_GolfSwing);
        }

        // Called by a UI Button.
        public void PlayController()
        {
            _Animancer.PlayController();
        }

        /************************************************************************************************************************/
    }
}
