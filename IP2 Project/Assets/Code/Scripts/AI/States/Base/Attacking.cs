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
        private EntityMovement _movementScript;


        [Header("Attacks")]
        [SerializeField] private Attack _attack;
        [SerializeField] private bool _predictTargetPosition;
        private Vector2 _previousTargetPosition;


        [Space(5)]
        [SerializeField] private float _maxAttackRange;
        private float _attackCooldownCompleteTime;


        [Header("Keep Distance")]
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;
        //public bool ShouldStopAttacking() => Vector2.Distance(_movementScript.transform.position, _targetPos()) > _maxAttackRange;
        public bool ShouldStopAttacking() => Physics2D.Linecast(_movementScript.transform.position, _targetPos());


        public void InitialiseValues(Func<Vector2> target, EntityMovement movementScript)
        {
            this._targetPos = target;
            this._movementScript = movementScript;
        }


        public override void OnLogic()
        {
            base.OnLogic();


            // Calculate the position where our target is expected to be when our attack will land.
            Vector2 targetPos = _targetPos();
            Vector2? interceptionPosition = _attack.CalculateInterceptionPosition(_movementScript.transform.position, targetPos, (targetPos - _previousTargetPosition) / Time.deltaTime);
            Vector2 estimatedTargetPos = interceptionPosition.HasValue ? interceptionPosition.Value : targetPos;
            _previousTargetPosition = targetPos;


            // Calculate the distance to the target.
            float distanceToTarget = Vector2.Distance(targetPos, _movementScript.transform.position);

            // If we are within range to attack, and our cooldown has elapsed, then make the attack.
            if (distanceToTarget < _maxAttackRange && Time.time >= _attackCooldownCompleteTime)
            {
                _attack.MakeAttack(_movementScript.transform, estimatedTargetPos);
                _attackCooldownCompleteTime = Time.time + _attack.GetRecoveryTime();
            }


            // Move & rotate towards the target with our behaviours set.
            _movementScript.CalculateMovement(targetPos, _movementBehaviours, RotationType.TargetDirection);
        }


        public void DrawGizmos(Transform gizmosOrigin, bool drawGizmos = true)
        {
            if (drawGizmos)
            {
                // Draw Max Attack Radius.
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(gizmosOrigin.position, _maxAttackRange);
            }

            // Movement Behaviour Gizmos.
            foreach (BaseSteeringBehaviour behaviour in _movementBehaviours)
                behaviour.DrawGizmos(gizmosOrigin);
        }
    }
}