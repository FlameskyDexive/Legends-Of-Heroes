using System;
using System.Diagnostics;
using System.Numerics;
using Box2DSharp.Common;

namespace Box2DSharp.Dynamics.Joints
{
    /// A revolute joint constrains two bodies to share a common point while they
    /// are free to rotate about the point. The relative rotation about the shared
    /// point is the joint angle. You can limit the relative rotation with
    /// a joint limit that specifies a lower and upper angle. You can use a motor
    /// to drive the relative rotation about the shared point. A maximum motor torque
    /// is provided so that infinite forces are not generated.
    public class RevoluteJoint : Joint
    {
        internal readonly float ReferenceAngle;

        private bool _enableLimit;

        private bool _enableMotor;

        private Vector2 _impulse;

        // Solver temp
        private int _indexA;

        private int _indexB;

        private float _invIa;

        private float _invIb;

        private Matrix2x2 _K;

        private float _angle;

        private float _axialMass;

        private float _invMassA;

        private float _invMassB;

        private Vector2 _localCenterA;

        private Vector2 _localCenterB;

        private float _lowerAngle;

        private float _maxMotorTorque;

        private float _motorImpulse;

        private float _lowerImpulse;

        private float _upperImpulse;

        private float _motorSpeed;

        private Vector2 _rA;

        private Vector2 _rB;

        private float _upperAngle;

        // Solver shared
        internal Vector2 LocalAnchorA;

        internal Vector2 LocalAnchorB;

        internal RevoluteJoint(RevoluteJointDef def)
            : base(def)
        {
            LocalAnchorA = def.LocalAnchorA;
            LocalAnchorB = def.LocalAnchorB;
            ReferenceAngle = def.ReferenceAngle;

            _impulse.SetZero();
            _motorImpulse = 0.0f;
            _axialMass = 0.0f;
            _lowerImpulse = 0.0f;
            _upperImpulse = 0.0f;

            _lowerAngle = def.LowerAngle;
            _upperAngle = def.UpperAngle;
            _maxMotorTorque = def.MaxMotorTorque;
            _motorSpeed = def.MotorSpeed;
            _enableLimit = def.EnableLimit;
            _enableMotor = def.EnableMotor;
            _angle = 0.0f;
        }

        /// The local anchor point relative to bodyA's origin.
        public Vector2 GetLocalAnchorA()
        {
            return LocalAnchorA;
        }

        /// The local anchor point relative to bodyB's origin.
        public Vector2 GetLocalAnchorB()
        {
            return LocalAnchorB;
        }

        /// Get the reference angle.
        public float GetReferenceAngle()
        {
            return ReferenceAngle;
        }

        /// Get the current joint angle in radians.
        public float GetJointAngle()
        {
            var bA = BodyA;
            var bB = BodyB;
            return bB.Sweep.A - bA.Sweep.A - ReferenceAngle;
        }

        /// Get the current joint angle speed in radians per second.
        public float GetJointSpeed()
        {
            var bA = BodyA;
            var bB = BodyB;
            return bB.AngularVelocity - bA.AngularVelocity;
        }

        /// Is the joint limit enabled?
        public bool IsLimitEnabled()
        {
            return _enableLimit;
        }

        /// Enable/disable the joint limit.
        public void EnableLimit(bool flag)
        {
            if (flag != _enableLimit)
            {
                BodyA.IsAwake = true;
                BodyB.IsAwake = true;
                _enableLimit = flag;
                _lowerImpulse = 0.0f;
                _upperImpulse = 0.0f;
            }
        }

        /// Get the lower joint limit in radians.
        public float GetLowerLimit()
        {
            return _lowerAngle;
        }

        /// Get the upper joint limit in radians.
        public float GetUpperLimit()
        {
            return _upperAngle;
        }

        /// Set the joint limits in radians.
        public void SetLimits(float lower, float upper)
        {
            Debug.Assert(lower <= upper);

            if (Math.Abs(lower - _lowerAngle) > Settings.Epsilon || Math.Abs(upper - _upperAngle) > Settings.Epsilon)
            {
                BodyA.IsAwake = true;
                BodyB.IsAwake = true;
                _lowerImpulse = 0.0f;
                _upperImpulse = 0.0f;
                _lowerAngle = lower;
                _upperAngle = upper;
            }
        }

        /// Is the joint motor enabled?
        public bool IsMotorEnabled()
        {
            return _enableMotor;
        }

        /// Enable/disable the joint motor.
        public void EnableMotor(bool flag)
        {
            if (flag != _enableMotor)
            {
                BodyA.IsAwake = true;
                BodyB.IsAwake = true;
                _enableMotor = flag;
            }
        }

        /// Set the motor speed in radians per second.
        public void SetMotorSpeed(float speed)
        {
            if (speed != _motorSpeed)
            {
                BodyA.IsAwake = true;
                BodyB.IsAwake = true;
                _motorSpeed = speed;
            }
        }

        /// Get the motor speed in radians per second.
        public float GetMotorSpeed()
        {
            return _motorSpeed;
        }

        /// Set the maximum motor torque, usually in N-m.
        public void SetMaxMotorTorque(float torque)
        {
            if (torque != _maxMotorTorque)
            {
                BodyA.IsAwake = true;
                BodyB.IsAwake = true;
                _maxMotorTorque = torque;
            }
        }

        public float GetMaxMotorTorque()
        {
            return _maxMotorTorque;
        }

        /// Get the reaction force given the inverse time step.
        /// Unit is N.
        /// Get the current motor torque given the inverse time step.
        /// Unit is N*m.
        public float GetMotorTorque(float inv_dt)
        {
            return inv_dt * _motorImpulse;
        }

        /// <inheritdoc />
        public override Vector2 GetAnchorA()
        {
            return BodyA.GetWorldPoint(LocalAnchorA);
        }

        /// <inheritdoc />
        public override Vector2 GetAnchorB()
        {
            return BodyB.GetWorldPoint(LocalAnchorB);
        }

        /// <inheritdoc />
        public override Vector2 GetReactionForce(float inv_dt)
        {
            var P = new Vector2(_impulse.X, _impulse.Y);
            return inv_dt * P;
        }

        /// <inheritdoc />
        public override float GetReactionTorque(float inv_dt)
        {
            return inv_dt * (_motorImpulse + _lowerImpulse - _upperImpulse);
        }

        /// Dump to Logger.Log.
        public override void Dump()
        {
            var indexA = BodyA.IslandIndex;
            var indexB = BodyB.IslandIndex;

            DumpLogger.Log("  b2RevoluteJointDef jd;");
            DumpLogger.Log($"  jd.bodyA = bodies[{indexA}];");
            DumpLogger.Log($"  jd.bodyB = bodies[{indexB}];");
            DumpLogger.Log($"  jd.collideConnected = bool({CollideConnected});");
            DumpLogger.Log($"  jd.localAnchorA.Set({LocalAnchorA.X}, {LocalAnchorA.Y});");
            DumpLogger.Log($"  jd.localAnchorB.Set({LocalAnchorB.X}, {LocalAnchorB.Y});");
            DumpLogger.Log($"  jd.referenceAngle = {ReferenceAngle};");
            DumpLogger.Log($"  jd.enableLimit = bool({_enableLimit});");
            DumpLogger.Log($"  jd.lowerAngle = {_lowerAngle};");
            DumpLogger.Log($"  jd.upperAngle = {_upperAngle};");
            DumpLogger.Log($"  jd.enableMotor = bool({_enableMotor});");
            DumpLogger.Log($"  jd.motorSpeed = {_motorSpeed};");
            DumpLogger.Log($"  jd.maxMotorTorque = {_maxMotorTorque};");
            DumpLogger.Log($"  joints[{Index}] = m_world.CreateJoint(&jd);");
        }

        /// <inheritdoc />
        internal override void InitVelocityConstraints(in SolverData data)
        {
            _indexA = BodyA.IslandIndex;
            _indexB = BodyB.IslandIndex;
            _localCenterA = BodyA.Sweep.LocalCenter;
            _localCenterB = BodyB.Sweep.LocalCenter;
            _invMassA = BodyA.InvMass;
            _invMassB = BodyB.InvMass;
            _invIa = BodyA.InverseInertia;
            _invIb = BodyB.InverseInertia;

            var aA = data.Positions[_indexA].Angle;
            var vA = data.Velocities[_indexA].V;
            var wA = data.Velocities[_indexA].W;

            var aB = data.Positions[_indexB].Angle;
            var vB = data.Velocities[_indexB].V;
            var wB = data.Velocities[_indexB].W;

            var qA = new Rotation(aA);
            var qB = new Rotation(aB);

            _rA = MathUtils.Mul(qA, LocalAnchorA - _localCenterA);
            _rB = MathUtils.Mul(qB, LocalAnchorB - _localCenterB);

            // J = [-I -r1_skew I r2_skew]
            //     [ 0       -1 0       1]
            // r_skew = [-ry; rx]

            // Matlab
            // K = [ mA+r1y^2*iA+mB+r2y^2*iB,  -r1y*iA*r1x-r2y*iB*r2x]
            //     [  -r1y*iA*r1x-r2y*iB*r2x, mA+r1x^2*iA+mB+r2x^2*iB]

            float mA = _invMassA, mB = _invMassB;
            float iA = _invIa, iB = _invIb;

            _K.Ex.X = mA + mB + _rA.Y * _rA.Y * iA + _rB.Y * _rB.Y * iB;
            _K.Ey.X = -_rA.Y * _rA.X * iA - _rB.Y * _rB.X * iB;
            _K.Ex.Y = _K.Ey.X;
            _K.Ey.Y = mA + mB + _rA.X * _rA.X * iA + _rB.X * _rB.X * iB;

            _axialMass = iA + iB;
            bool fixedRotation;
            if (_axialMass > 0.0f)
            {
                _axialMass = 1.0f / _axialMass;
                fixedRotation = false;
            }
            else
            {
                fixedRotation = true;
            }

            _angle = aB - aA - ReferenceAngle;
            if (_enableLimit == false || fixedRotation)
            {
                _lowerImpulse = 0.0f;
                _upperImpulse = 0.0f;
            }

            if (_enableMotor == false || fixedRotation)
            {
                _motorImpulse = 0.0f;
            }

            if (data.Step.WarmStarting)
            {
                // Scale impulses to support a variable time step.
                _impulse *= data.Step.DtRatio;
                _motorImpulse *= data.Step.DtRatio;
                _lowerImpulse *= data.Step.DtRatio;
                _upperImpulse *= data.Step.DtRatio;

                var axialImpulse = _motorImpulse + _lowerImpulse - _upperImpulse;
                var P = new Vector2(_impulse.X, _impulse.Y);

                vA -= mA * P;
                wA -= iA * (MathUtils.Cross(_rA, P) + axialImpulse);

                vB += mB * P;
                wB += iB * (MathUtils.Cross(_rB, P) + axialImpulse);
            }
            else
            {
                _impulse.SetZero();
                _motorImpulse = 0.0f;
                _lowerImpulse = 0.0f;
                _upperImpulse = 0.0f;
            }

            data.Velocities[_indexA].V = vA;
            data.Velocities[_indexA].W = wA;
            data.Velocities[_indexB].V = vB;
            data.Velocities[_indexB].W = wB;
        }

        /// <inheritdoc />
        internal override void SolveVelocityConstraints(in SolverData data)
        {
            var vA = data.Velocities[_indexA].V;
            var wA = data.Velocities[_indexA].W;
            var vB = data.Velocities[_indexB].V;
            var wB = data.Velocities[_indexB].W;

            float mA = _invMassA, mB = _invMassB;
            float iA = _invIa, iB = _invIb;

            var fixedRotation = (iA + iB).Equals(0.0f);

            // Solve motor constraint.
            if (_enableMotor && fixedRotation == false)
            {
                var cdot = wB - wA - _motorSpeed;
                var impulse = -_axialMass * cdot;
                var oldImpulse = _motorImpulse;
                var maxImpulse = data.Step.Dt * _maxMotorTorque;
                _motorImpulse = MathUtils.Clamp(_motorImpulse + impulse, -maxImpulse, maxImpulse);
                impulse = _motorImpulse - oldImpulse;

                wA -= iA * impulse;
                wB += iB * impulse;
            }

            if (_enableLimit && fixedRotation == false)
            {
                // Lower limit
                {
                    float C = _angle - _lowerAngle;
                    float Cdot = wB - wA;
                    float impulse = -_axialMass * (Cdot + Math.Max(C, 0.0f) * data.Step.InvDt);
                    float oldImpulse = _lowerImpulse;
                    _lowerImpulse = Math.Max(_lowerImpulse + impulse, 0.0f);
                    impulse = _lowerImpulse - oldImpulse;

                    wA -= iA * impulse;
                    wB += iB * impulse;
                }

                // Upper limit
                // Note: signs are flipped to keep C positive when the constraint is satisfied.
                // This also keeps the impulse positive when the limit is active.
                {
                    float C = _upperAngle - _angle;
                    float Cdot = wA - wB;
                    float impulse = -_axialMass * (Cdot + Math.Max(C, 0.0f) * data.Step.InvDt);
                    float oldImpulse = _upperImpulse;
                    _upperImpulse = Math.Max(_upperImpulse + impulse, 0.0f);
                    impulse = _upperImpulse - oldImpulse;

                    wA += iA * impulse;
                    wB -= iB * impulse;
                }
            }

            // Solve point-to-point constraint
            {
                var Cdot = vB + MathUtils.Cross(wB, _rB) - vA - MathUtils.Cross(wA, _rA);
                var impulse = _K.Solve(-Cdot);

                _impulse.X += impulse.X;
                _impulse.Y += impulse.Y;

                vA -= mA * impulse;
                wA -= iA * MathUtils.Cross(_rA, impulse);

                vB += mB * impulse;
                wB += iB * MathUtils.Cross(_rB, impulse);
            }

            data.Velocities[_indexA].V = vA;
            data.Velocities[_indexA].W = wA;
            data.Velocities[_indexB].V = vB;
            data.Velocities[_indexB].W = wB;
        }

        /// <inheritdoc />
        internal override bool SolvePositionConstraints(in SolverData data)
        {
            var cA = data.Positions[_indexA].Center;
            var aA = data.Positions[_indexA].Angle;
            var cB = data.Positions[_indexB].Center;
            var aB = data.Positions[_indexB].Angle;

            var qA = new Rotation(aA);
            var qB = new Rotation(aB);

            var angularError = 0.0f;
            var positionError = 0.0f;

            var fixedRotation = (_invIa + _invIb).Equals(0.0f);

            // Solve angular limit constraint
            if (_enableLimit && fixedRotation == false)
            {
                float angle = aB - aA - ReferenceAngle;
                float C = 0.0f;

                if (Math.Abs(_upperAngle - _lowerAngle) < 2.0f * Settings.AngularSlop)
                {
                    // Prevent large angular corrections
                    C = MathUtils.Clamp(angle - _lowerAngle, -Settings.MaxAngularCorrection, Settings.MaxAngularCorrection);
                }
                else if (angle <= _lowerAngle)
                {
                    // Prevent large angular corrections and allow some slop.
                    C = MathUtils.Clamp(angle - _lowerAngle + Settings.AngularSlop, -Settings.MaxAngularCorrection, 0.0f);
                }
                else if (angle >= _upperAngle)
                {
                    // Prevent large angular corrections and allow some slop.
                    C = MathUtils.Clamp(angle - _upperAngle - Settings.AngularSlop, 0.0f, Settings.MaxAngularCorrection);
                }

                float limitImpulse = -_axialMass * C;
                aA -= _invIa * limitImpulse;
                aB += _invIb * limitImpulse;
                angularError = Math.Abs(C);
            }

            // Solve point-to-point constraint.
            {
                qA.Set(aA);
                qB.Set(aB);
                var rA = MathUtils.Mul(qA, LocalAnchorA - _localCenterA);
                var rB = MathUtils.Mul(qB, LocalAnchorB - _localCenterB);

                var C = cB + rB - cA - rA;
                positionError = C.Length();

                float mA = _invMassA, mB = _invMassB;
                float iA = _invIa, iB = _invIb;

                var K = new Matrix2x2();
                K.Ex.X = mA + mB + iA * rA.Y * rA.Y + iB * rB.Y * rB.Y;
                K.Ex.Y = -iA * rA.X * rA.Y - iB * rB.X * rB.Y;
                K.Ey.X = K.Ex.Y;
                K.Ey.Y = mA + mB + iA * rA.X * rA.X + iB * rB.X * rB.X;

                var impulse = -K.Solve(C);

                cA -= mA * impulse;
                aA -= iA * MathUtils.Cross(rA, impulse);

                cB += mB * impulse;
                aB += iB * MathUtils.Cross(rB, impulse);
            }

            data.Positions[_indexA].Center = cA;
            data.Positions[_indexA].Angle = aA;
            data.Positions[_indexB].Center = cB;
            data.Positions[_indexB].Angle = aB;

            return positionError <= Settings.LinearSlop && angularError <= Settings.AngularSlop;
        }

        /// <inheritdoc />
        public override void Draw(IDrawer drawer)
        {
            var xfA = BodyA.GetTransform();
            var xfB = BodyB.GetTransform();
            var pA = MathUtils.Mul(xfA, LocalAnchorA);
            var pB = MathUtils.Mul(xfB, LocalAnchorB);

            var c1 = Color.FromArgb(0.7f, 0.7f, 0.7f);
            var c2 = Color.FromArgb(0.3f, 0.9f, 0.3f);
            var c3 = Color.FromArgb(0.9f, 0.3f, 0.3f);
            var c4 = Color.FromArgb(0.3f, 0.3f, 0.9f);
            var c5 = Color.FromArgb(0.4f, 0.4f, 0.4f);

            drawer.DrawPoint(pA, 5.0f, c4);
            drawer.DrawPoint(pB, 5.0f, c5);

            var aA = BodyA.GetAngle();
            var aB = BodyB.GetAngle();
            var angle = aB - aA - ReferenceAngle;

            var L = 0.5f;

            var r = L * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            drawer.DrawSegment(pB, pB + r, c1);
            drawer.DrawCircle(pB, L, c1);

            if (_enableLimit)
            {
                var rlo = L * new Vector2((float)Math.Cos(_lowerAngle), (float)Math.Cos(_lowerAngle));
                var rhi = L * new Vector2((float)Math.Cos(_upperAngle), (float)Math.Cos(_upperAngle));

                drawer.DrawSegment(pB, pB + rlo, c2);
                drawer.DrawSegment(pB, pB + rhi, c3);
            }

            var color = Color.FromArgb(0.5f, 0.8f, 0.8f);
            drawer.DrawSegment(xfA.Position, pA, color);
            drawer.DrawSegment(pA, pB, color);
            drawer.DrawSegment(xfB.Position, pB, color);
        }
    }
}