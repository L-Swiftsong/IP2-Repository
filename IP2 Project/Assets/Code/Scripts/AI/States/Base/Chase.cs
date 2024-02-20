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


        // These variable should never be edited outside of the 'InitialiseValues' function.
        private Func<Vector2> _targetPosition;
        private EntityMovement _movementScript;


        // Movement Params.
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;

        [SerializeField] private bool _faceTarget = false;


        // Initialise the Chase State's values.
        //  These values would ordinarily be readonly and set via constructor, but due to the method of creating the states we are testing to allow for runtime editing of parameters, we cannot do that.
        public void InitialiseValues(Func<Vector2> target, EntityMovement movementScript)
        {
            this._targetPosition = target;
            this._movementScript = movementScript;
        }


        public override void OnLogic()
        {
            base.OnLogic();

            // Move towards the target using our desired behaviours.
            _movementScript.CalculateMovement(_targetPosition(), _movementBehaviours, rotationType: _faceTarget ? RotationType.TargetDirection : RotationType.VelocityDirection);
        }


        public void DrawGizmos(Transform gizmoOrigin, bool drawGizmos = true)
        {
            foreach (BaseSteeringBehaviour behaviour in _movementBehaviours)
                behaviour.DrawGizmos(gizmoOrigin);
        }
    }
}