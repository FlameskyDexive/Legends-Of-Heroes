using System;
using System.Numerics;
using Box2DSharp.Common;

namespace Box2DSharp.Dynamics.Joints
{
    /// Distance joint definition. This requires defining an anchor point on both
    /// bodies and the non-zero distance of the distance joint. The definition uses
    /// local anchor points so that the initial configuration can violate the
    /// constraint slightly. This helps when saving and loading a game.
    public class DistanceJointDef : JointDef
    {
        /// Minimum length. Clamped to a stable minimum value.
        public float MinLength;

        /// Maximum length. Must be greater than or equal to the minimum length.
        public float MaxLength;

        /// The linear stiffness in N/m.
        public float Stiffness;

        /// The linear damping in N*s/m.
        public float Damping;

        /// The rest length of this joint. Clamped to a stable minimum value.
        public float Length;

        /// The local anchor point relative to bodyA's origin.
        public Vector2 LocalAnchorA;

        /// The local anchor point relative to bodyB's origin.
        public Vector2 LocalAnchorB;

        public DistanceJointDef()
        {
            JointType = JointType.DistanceJoint;
            LocalAnchorA.Set(0.0f, 0.0f);
            LocalAnchorB.Set(0.0f, 0.0f);
            MinLength = 0.0f;
            MaxLength = Settings.MaxFloat;
            Length = 1.0f;
            Stiffness = 0.0f;
            Damping = 0.0f;
        }

        /// Initialize the bodies, anchors, and rest length using world space anchors.
        /// The minimum and maximum lengths are set to the rest length.
        public void Initialize(
            Body b1,
            Body b2,
            in Vector2 anchor1,
            in Vector2 anchor2)
        {
            BodyA = b1;
            BodyB = b2;
            LocalAnchorA = BodyA.GetLocalPoint(anchor1);
            LocalAnchorB = BodyB.GetLocalPoint(anchor2);
            var d = anchor2 - anchor1;
            Length = Math.Max(d.Length(), Settings.LinearSlop);
            MinLength = Length;
            MaxLength = Length;
        }
    }
}