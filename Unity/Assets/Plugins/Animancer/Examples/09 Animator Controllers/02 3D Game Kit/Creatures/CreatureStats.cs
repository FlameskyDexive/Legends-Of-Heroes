// Animancer // Copyright 2020 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>The numerical details of a <see cref="Creature"/>.</summary>
    [Serializable]
    public sealed class CreatureStats
    {
        /************************************************************************************************************************/

        [SerializeField]
        private float _MaxSpeed = 8;
        public float MaxSpeed { get { return _MaxSpeed; } }

        [SerializeField]
        private float _Acceleration = 20;
        public float Acceleration { get { return _Acceleration; } }

        [SerializeField]
        private float _Deceleration = 25;
        public float Deceleration { get { return _Deceleration; } }

        [SerializeField]
        private float _MinTurnSpeed = 400;
        public float MinTurnSpeed { get { return _MinTurnSpeed; } }

        [SerializeField]
        private float _MaxTurnSpeed = 1200;
        public float MaxTurnSpeed { get { return _MaxTurnSpeed; } }

        [SerializeField]
        private float _Gravity = 20;
        public float Gravity { get { return _Gravity; } }

        [SerializeField]
        private float _StickingGravityProportion = 0.3f;
        public float StickingGravityProportion { get { return _StickingGravityProportion; } }

        /************************************************************************************************************************/
    }
}
