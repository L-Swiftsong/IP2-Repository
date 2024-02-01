using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Patrol : State
    {
        public override string Name { get => "Patrolling"; }


        [SerializeField] private List<Vector2> _patrolPoints;
        [SerializeField] private float _patrolSpeed;
    }
}