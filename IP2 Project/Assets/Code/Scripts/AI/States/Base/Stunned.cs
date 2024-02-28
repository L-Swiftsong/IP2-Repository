using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;

namespace States.Base
{
    [System.Serializable]
    public class Stunned : State
    {
        public override string Name { get => "Stunned"; }


        [Tooltip("The minimum amount of damage required for the enemy to be stunned by an attack")]
        [SerializeField, Min(1)] private int _stunThreshold = 1;
        public int StunThreshold => _stunThreshold;

        [Tooltip("The percentage chance of this entity to resist being stunned")]
        [SerializeField, Range(0f, 100f)] private float _stunResistance = 0f;
        public float StunResistance => _stunResistance;


        [SerializeField] private float _stunDuration;
        private float _stunCompleteTime;

        public bool HasStunCompleted => Time.time >= _stunCompleteTime;


        public override void OnEnter() => _stunCompleteTime = Time.time + _stunDuration;

        /// <returns> True if we have resisted the stun, or false if we have not.</returns>
        public bool HasResistedStun()
        {
            // Get a random percentage chance. If this change is above our resistance, we failed to resist the stun.
            float requiredResistancePercent = Random.Range(0f, 100f);
            return _stunResistance > requiredResistancePercent;
        }
    }
}