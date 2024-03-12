using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;
using System;

namespace States.Base
{
    [System.Serializable]
    public class Investigate : State
    {
        public override string Name { get => "Investigate"; }


        private EntityMovement _movementScript;
        private Func<Vector2> _pointOfInterest;

        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;
        [SerializeField] private float _investigationRadius = 0.75f;


        public void InitialiseValues(EntityMovement movementScript, Func<Vector2> pointOfInterest)
        {
            this._movementScript = movementScript;
            this._pointOfInterest = pointOfInterest;
        }

        public override void OnLogic()
        {
            base.OnLogic();

            // Move to point of interest.
            _movementScript.CalculateMovement(_pointOfInterest(), _movementBehaviours, rotationType: RotationType.VelocityDirection);
        }


        public bool WithinInvestigationRadius() => Vector2.Distance(_movementScript.transform.position, _pointOfInterest()) <= _investigationRadius;
    }
}