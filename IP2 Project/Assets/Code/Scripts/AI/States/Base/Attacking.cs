using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


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
        [SerializeField] private bool _predictTargetPosition;
        private Vector2 _previousTargetPosition;


        [Space(5)]
        [SerializeField] private float _maxAttackRange;
        private float _attackCooldownCompleteTime;


        [Header("Keep Distance")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotationSpeed;
        
        [Space(5)]
        [SerializeField] private float _keepDistance;
        [SerializeField] private float _maxDistance;
        [SerializeField, Range(0f, 1f)] private float _smoothingPercent;
        public float MaxDistance => _maxDistance;


        public void InitialiseValues(Func<Vector2> target, Transform movementTransform)
        {
            this._targetPos = target;
            this._movementTransform = movementTransform;
        }


        public override void OnLogic()
        {
            base.OnLogic();


            // Calculate the position where our target is expected to be when our attack will land.
            Vector2 targetPos = _targetPos();
            Vector2? interceptionPosition = _attack.CalculateInterceptionPosition(_movementTransform.position, targetPos, (targetPos - _previousTargetPosition) / Time.deltaTime);
            Vector2 estimatedTargetPos = interceptionPosition.HasValue ? interceptionPosition.Value : targetPos;
            _previousTargetPosition = targetPos;


            Vector2 targetDirection = (estimatedTargetPos - (Vector2)_movementTransform.position).normalized;
            float distanceToTarget = Vector2.Distance(targetPos, _movementTransform.position);


            // If we are within range to attack, and our cooldown has elapsed, then make the attack.
            if (distanceToTarget < _maxAttackRange && Time.time >= _attackCooldownCompleteTime)
            {
                _attack.MakeAttack(_movementTransform, estimatedTargetPos);
                _attackCooldownCompleteTime = Time.time + _attack.GetRecoveryTime();
            }


            // Calculate values for keeping distance.
            Vector2 directionToMove = CalculateDirectionToMove(targetDirection, distanceToTarget);

            // Move to keep distance.
            _movementTransform.position += (Vector3)directionToMove * _moveSpeed * Time.deltaTime;

            // Face target.
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
            _movementTransform.rotation = Quaternion.RotateTowards(_movementTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private Vector2 CalculateDirectionToMove(Vector2 directionToTarget, float distanceToTarget)
        {
            Vector2 directionToMove = Vector2.zero;
            float targetDistance = (_maxDistance + _keepDistance) / 2f;
            float smoothingThreshold = _smoothingPercent * (_maxDistance - _keepDistance) / 2f;

            // Determine the movement vector & strength of this vector for keeping distance to the target.
            if (distanceToTarget > targetDistance + smoothingThreshold)
                directionToMove = directionToTarget * Mathf.Lerp(0f, 1f, distanceToTarget - targetDistance);
            else if (distanceToTarget < targetDistance - smoothingThreshold)
                directionToMove = -directionToTarget * Mathf.Lerp(0f, 1f, targetDistance - distanceToTarget);

            return directionToMove;
        }


        public void DrawGizmos(Transform gizmosOrigin)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gizmosOrigin.position, _keepDistance); // (Gizmo) Draw 'Keep Distance' Radius.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gizmosOrigin.position, _maxDistance); // (Gizmo) Draw 'Max Distance' Radius.


            // (Gizmo) Draw Max Attack Radius.
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gizmosOrigin.position, _maxAttackRange);


            // (Gizmo) Draw Threshold Radii.
            Gizmos.color = Color.blue;
            float targetDistance = (_maxDistance + _keepDistance) / 2f;
            float smoothingValue = (_smoothingPercent * (_maxDistance - _keepDistance)) / 2f;
            Gizmos.DrawWireSphere(gizmosOrigin.position, targetDistance + smoothingValue);
            Gizmos.DrawWireSphere(gizmosOrigin.position, targetDistance - smoothingValue);
        }
    }
}