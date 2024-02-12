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


        [Header("Attacks")]
        [SerializeField] private Attack _attack;
        [SerializeField] private float _maxAttackRange;
        private float _attackCooldownCompleteTime;


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


        public void InitialiseValues(Func<Vector2> target, Transform movementTransform)
        {
            this._targetPos = target;
            this._movementTransform = movementTransform;
        }


        public override void OnLogic()
        {
            base.OnLogic();


            Vector2 targetDirection = _targetPos() - (Vector2)_movementTransform.position;
            float distanceToTarget = targetDirection.magnitude;
            targetDirection.Normalize();


            // If we are within range to attack, and our cooldown has elapsed, then make the attack.
            if (distanceToTarget < _maxAttackRange && Time.time >= _attackCooldownCompleteTime)
            {
                _attack.MakeAttack(_movementTransform, _targetPos());
                _attackCooldownCompleteTime = Time.time + _attack.GetRecoveryTime();
            }


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