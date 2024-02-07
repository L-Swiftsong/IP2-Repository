using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;
using System;

namespace States.Base
{
    [System.Serializable]
    public class Attacking : State
    {
        public override string Name { get => "Attacking"; }


        private Func<Vector2> _targetPos;
        private Transform _movementTransform;


        [Header("Keep Distance")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotationSpeed;
        
        [Space(5)]
        [SerializeField] private float _keepDistance;
        [SerializeField] private float _maxDistance;
        [SerializeField, Range(0f, 1f)] private float _smoothingPercent;
        public float MaxAttackDistance => _maxDistance;
        public float KeepDistance => _keepDistance; // For Debug.
        public float SmoothingThreshold => _smoothingPercent * (_maxDistance - _keepDistance) / 2f; // For Debug;



        [Header("Example Attack Variables")]
        [SerializeField] private float _exampleAttackRecovery;
        private float _attackRecoveryCompleteTime;


        public void InitialiseValues(Func<Vector2> target, Transform movementTransform)
        {
            this._targetPos = target;
            this._movementTransform = movementTransform;
        }


        public override void OnLogic()
        {
            base.OnLogic();

            // If we are still recovering from an attack, stop here.
            if (Time.time < _attackRecoveryCompleteTime)
                return;


            Vector2 targetDirection = _targetPos() - (Vector2)_movementTransform.position;
            float distanceToTarget = targetDirection.magnitude;
            targetDirection.Normalize();

            // Calculate values for keeping distance.
            Vector2 directionToMove = Vector2.zero;
            float targetDistance = (_maxDistance + _keepDistance) / 2f;
            float smoothingThreshold = _smoothingPercent * (_maxDistance - _keepDistance) / 2f;

            // Determine the movement vector & strength of this vector for keeping distance to the target.
            if (distanceToTarget > targetDistance + smoothingThreshold)
                directionToMove = targetDirection * Mathf.Lerp(0f, 1f, distanceToTarget - targetDistance);
            else if (distanceToTarget < targetDistance - smoothingThreshold)
                directionToMove = -targetDirection * Mathf.Lerp(0f, 1f, targetDistance - distanceToTarget);

            // Move to keep distance.
            _movementTransform.position += (Vector3)directionToMove * _moveSpeed * Time.deltaTime;


            // Face target.
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
            _movementTransform.rotation = Quaternion.RotateTowards(_movementTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}