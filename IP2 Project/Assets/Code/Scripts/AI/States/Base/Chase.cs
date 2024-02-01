using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Chase : State
    {
        public override string Name { get => "Chasing"; }


        // Readonly Variables.
        private readonly Func<Vector2> _target;


        // Movement Params.
        [SerializeField] private float _chaseSpeed;


        // Constructor.
        public Chase(Func<Vector2> target) => _target = target;
    }
}