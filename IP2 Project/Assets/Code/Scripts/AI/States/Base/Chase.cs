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
        private Transform _movementTransform;


        // Movement Params.
        [SerializeField] private float _chaseSpeed;

        [SerializeField] private bool _faceTarget = false;
        [SerializeField] private float _rotationSpeed;
        private Vector2 _previousPosition;


        // Initialise the Chase State's values.
        //  These values would ordinarily be readonly and set via constructor, but due to the method of creating the states we are testing to allow for runtime editing of parameters, we cannot do that.
        public void InitialiseValues(Func<Vector2> target, Transform movementTransform)
        {
            this._targetPosition = target;
            this._movementTransform = movementTransform;
        }


        public override void OnEnter()
        {
            base.OnEnter();

            _previousPosition = _movementTransform.position;
        }
        public override void OnLogic()
        {
            base.OnLogic();

            // Calculate the desired direction.
            Vector2 targetDirection = (_targetPosition() - (Vector2)_movementTransform.position).normalized;
            
            // Move towards the target.
            _movementTransform.position += (Vector3)targetDirection * _chaseSpeed * Time.deltaTime;

            // Calculate our target rotation.
            Quaternion targetRotation;
            if (_faceTarget)
                targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
            else
            {
                targetRotation = Quaternion.LookRotation(Vector3.forward, ((Vector2)_movementTransform.position - _previousPosition).normalized);
                _previousPosition = _movementTransform.position;
            }

            // Rotate to face the target direction.
            _movementTransform.rotation = Quaternion.RotateTowards(_movementTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}