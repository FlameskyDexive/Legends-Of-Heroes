// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.InverseKinematics
{
    /// <summary>
    /// Demonstrates how to use Unity's Inverse Kinematics (IK) system to adjust a character's feet according to the
    /// terrain they are moving over.
    /// </summary>
    [AddComponentMenu(Strings.MenuPrefix + "Examples/Inverse Kinematics - Raycast Foot IK")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.InverseKinematics/RaycastFootIK")]
    public sealed class RaycastFootIK : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private ExposedCurve _LeftFootWeight;
        [SerializeField] private ExposedCurve _RightFootWeight;
        [SerializeField] private float _RaycastOriginY = 0.5f;
        [SerializeField] private float _RaycastEndY = -0.2f;

        /************************************************************************************************************************/

        private Transform _LeftFoot;
        private Transform _RightFoot;

        /************************************************************************************************************************/

        /// <summary>Public property for a UI Toggle to enable or disable the IK.</summary>
        public bool ApplyAnimatorIK
        {
            get { return _Animancer.Layers[0].ApplyAnimatorIK; }
            set { _Animancer.Layers[0].ApplyAnimatorIK = value; }
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            // Make sure both curves are actually being extracted from the same animation.
            Debug.Assert(_LeftFootWeight.Clip == _RightFootWeight.Clip);

            // Play that animation (ExposedCurve has an implicit cast to use its Clip).
            _Animancer.Play(_LeftFootWeight);

            // Tell Unity that OnAnimatorIK needs to be called every frame.
            ApplyAnimatorIK = true;

            // Get the foot bones.
            _LeftFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            _RightFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }

        /************************************************************************************************************************/

        // Note that due to limitations in the Playables API, Unity will always call this method with layerIndex = 0.
#pragma warning disable IDE0060 // Remove unused parameter.
        private void OnAnimatorIK(int layerIndex)
#pragma warning restore IDE0060 // Remove unused parameter.
        {
            UpdateFootIK(AvatarIKGoal.LeftFoot, _LeftFootWeight, _LeftFoot, _Animancer.Animator.leftFeetBottomHeight);
            UpdateFootIK(AvatarIKGoal.RightFoot, _RightFootWeight, _RightFoot, _Animancer.Animator.rightFeetBottomHeight);
        }

        /************************************************************************************************************************/

        private void UpdateFootIK(AvatarIKGoal goal, ExposedCurve curve, Transform footTransform, float footBottomHeight)
        {
            var weight = curve.Evaluate(_Animancer);
            _Animancer.Animator.SetIKPositionWeight(goal, weight);
            _Animancer.Animator.SetIKRotationWeight(goal, weight);

            if (weight == 0)
                return;

            // Get the local up direction of the foot.
            var rotation = _Animancer.Animator.GetIKRotation(goal);
            var localUp = rotation * Vector3.up;

            var position = footTransform.position;
            position += localUp * _RaycastOriginY;

            var distance = _RaycastOriginY - _RaycastEndY;

            RaycastHit hit;
            if (Physics.Raycast(position, -localUp, out hit, distance))
            {
                // Use the hit point as the desired position.
                position = hit.point;
                position += localUp * footBottomHeight;
                _Animancer.Animator.SetIKPosition(goal, position);

                // Use the hit normal to calculate the desired rotation.
                var rotAxis = Vector3.Cross(localUp, hit.normal);
                var angle = Vector3.Angle(localUp, hit.normal);
                rotation = Quaternion.AngleAxis(angle, rotAxis) * rotation;

                _Animancer.Animator.SetIKRotation(goal, rotation);
            }
            // Otherwise simply stretch the leg out to the end of the ray.
            else
            {
                position += localUp * (footBottomHeight - distance);
                _Animancer.Animator.SetIKPosition(goal, position);
            }
        }

        /************************************************************************************************************************/
    }
}
