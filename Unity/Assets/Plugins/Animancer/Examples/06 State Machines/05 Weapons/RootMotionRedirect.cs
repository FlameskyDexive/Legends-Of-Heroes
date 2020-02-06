// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Weapons
{
    /// <summary>
    /// Takes the root motion from the <see cref="Animator"/> attached to the same <see cref="GameObject"/> and applies
    /// it to a <see cref="Rigidbody"/> on a different object.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Weapons - Root Motion Redirect")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Weapons/RootMotionRedirect")]
    public sealed class RootMotionRedirect : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private Rigidbody _Rigidbody;
        [SerializeField] private Animator _Animator;

        /************************************************************************************************************************/

        private void OnAnimatorMove()
        {
            if (_Animator.applyRootMotion)
            {
                _Rigidbody.MovePosition(_Rigidbody.position + _Animator.deltaPosition);
                _Rigidbody.MoveRotation(_Rigidbody.rotation * _Animator.deltaRotation);
            }
        }

        /************************************************************************************************************************/
    }
}
