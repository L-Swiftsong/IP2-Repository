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
        [SerializeField, Min(1)] private int _stunThreshold;
        public int StunThreshold => _stunThreshold;

        [SerializeField] private float _stunDuration;
        private float _stunCompleteTime;

        public bool HasStunCompleted => Time.time >= _stunCompleteTime;


        public override void OnEnter() => _stunCompleteTime = Time.time + _stunDuration;
    }
}