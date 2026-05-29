namespace Box2DSharp.Common
{
    public static class Settings
    {
        public const float MaxFloat = float.MaxValue;

        public const float Epsilon = 1.192092896e-7f;

        public const float Pi = 3.14159265359f;

        public const float LengthUnitsPerMeter = 1.0f;

        // @file
        // Global tuning constants based on meters-kilograms-seconds (MKS) units.

        // Collision

        /// The maximum number of contact points between two convex shapes. Do
        /// not change this value.
        public const int MaxManifoldPoints = 2;

        /// The maximum number of vertices on a convex polygon. You cannot increase
        /// this too much because b2BlockAllocator has a maximum object size.
        public const int MaxPolygonVertices = 8;

        /// This is used to fatten AABBs in the dynamic tree. This allows proxies
        /// to move by a small amount without triggering a tree adjustment.
        /// This is in meters.
        public const float AABBExtension = 0.1f * LengthUnitsPerMeter;

        /// This is used to fatten AABBs in the dynamic tree. This is used to predict
        /// the future position based on the current displacement.
        /// This is a dimensionless multiplier.
        public const float AABBMultiplier = 4.0f;

        /// A small length used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        public const float LinearSlop = 0.005f * LengthUnitsPerMeter;

        /// A small angle used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        public const float AngularSlop = 2.0f / 180.0f * Pi;

        /// The radius of the polygon/edge shape skin. This should not be modified. Making
        /// this smaller means polygons will have an insufficient buffer for continuous collision.
        /// Making it larger may create artifacts for vertex collision.
        public const float PolygonRadius = 2.0f * LinearSlop;

        /// Maximum number of sub-steps per contact in continuous physics simulation.
        public const int MaxSubSteps = 8;

        // Dynamics

        /// Maximum number of contacts to be handled to solve a TOI impact.
        public const int MaxToiContacts = 32;

        /// The maximum linear position correction used when solving constraints. This helps to
        /// prevent overshoot.
        public const float MaxLinearCorrection = 0.2f * LengthUnitsPerMeter;

        /// The maximum angular position correction used when solving constraints. This helps to
        /// prevent overshoot.
        public const float MaxAngularCorrection = 8.0f / 180.0f * Pi;

        /// The maximum linear velocity of a body. This limit is very large and is used
        /// to prevent numerical problems. You shouldn't need to adjust this.
        public const float MaxTranslation = 2.0f * LengthUnitsPerMeter;

        public const float MaxTranslationSquared = MaxTranslation * MaxTranslation;

        /// The maximum angular velocity of a body. This limit is very large and is used
        /// to prevent numerical problems. You shouldn't need to adjust this.
        public const float MaxRotation = 0.5f * Pi;

        public const float MaxRotationSquared = MaxRotation * MaxRotation;

        /// This scale factor controls how fast overlap is resolved. Ideally this would be 1 so
        /// that overlap is removed in one time step. However using values close to 1 often lead
        /// to overshoot.
        public const float Baumgarte = 0.2f;

        public const float ToiBaumgarte = 0.75f;

        // Sleep

        /// The time that a body must be still before it will go to sleep.
        public const float TimeToSleep = 0.5f;

        /// A body cannot sleep if its linear velocity is above this tolerance.
        public const float LinearSleepTolerance = 0.01f * LengthUnitsPerMeter;

        /// A body cannot sleep if its angular velocity is above this tolerance.
        public const float AngularSleepTolerance = 2.0f / 180.0f * Pi;
    }
}