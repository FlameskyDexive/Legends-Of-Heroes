using System.Numerics;
using Box2DSharp.Common;

namespace Box2DSharp.Dynamics.Joints
{
    /// Weld joint definition. You need to specify local anchor points
    /// where they are attached and the relative body angle. The position
    /// of the anchor points is important for computing the reaction torque.
    public class WeldJointDef : JointDef
    {
        /// The rotational stiffness in N*m
        /// Disable softness with a value of 0
        public float Stiffness;

        /// The rotational damping in N*m*s
        public float Damping;

        /// The local anchor point relative to bodyA's origin.
        public Vector2 LocalAnchorA;

        /// The local anchor point relative to bodyB's origin.
        public Vector2 LocalAnchorB;

        /// The bodyB angle minus bodyA angle in the reference state (radians).
        public float ReferenceAngle;

        public WeldJointDef()
        {
            JointType = JointType.WeldJoint;
            LocalAnchorA.Set(0.0f, 0.0f);
            LocalAnchorB.Set(0.0f, 0.0f);
            ReferenceAngle = 0.0f;
            Stiffness = 0.0f;
            Damping = 0.0f;
        }

        /// <summary>
        /// Initialize the bodies, anchors, reference angle, stiffness, and damping.
        /// </summary>
        /// <param name="bA">the first body connected by this joint</param>
        /// <param name="bB">the second body connected by this joint</param>
        /// <param name="anchor">the point of connection in world coordinates</param>
        public void Initialize(Body bA, Body bB, in Vector2 anchor)
        {
            BodyA = bA;
            BodyB = bB;
            LocalAnchorA = BodyA.GetLocalPoint(anchor);
            LocalAnchorB = BodyB.GetLocalPoint(anchor);
            ReferenceAngle = BodyB.GetAngle() - BodyA.GetAngle();
        }
    }
}