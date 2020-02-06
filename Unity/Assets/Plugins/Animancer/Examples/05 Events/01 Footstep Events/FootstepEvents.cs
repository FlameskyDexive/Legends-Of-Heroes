// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// Uses Animancer Events to play a sound randomly selected from an array.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Footstep Events - Animancer")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimationEvents/FootstepEvents")]
    public class FootstepEvents : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private ClipState.Transition _Walk;
        [SerializeField] private AudioClip[] _Sounds;

        /************************************************************************************************************************/

        protected void OnEnable()
        {
            _Animancer.Play(_Walk);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Called by Animancer Events. Chooses a random sound an plays it on the specified `source` (because each foot
        /// has its own <see cref="AudioSource"/>).
        /// </summary>
        public void PlaySound(AudioSource source)
        {
            source.clip = _Sounds[Random.Range(0, _Sounds.Length)];
            source.Play();

            // Note that the minimum value in Random.Range is inclusive (so it can pick 0) while the maximum value is
            // exclusive (so it can not pick `_Sounds.Length`), which is perfect for picking a random array element.

            // A more complex system could have different footstep sounds depending on the surface being stepped on.
            // This could be done by raycasting down from the feet and determining which sound to use based on the
            // sharedMaterial of the ground's Renderer as demonstrated in the 3D Game Kit example or even a simple
            // script that holds an enum indicating the type.
        }

        /************************************************************************************************************************/
    }
}
