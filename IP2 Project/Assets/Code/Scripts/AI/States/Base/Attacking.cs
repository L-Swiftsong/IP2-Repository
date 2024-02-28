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
        [SerializeField] private Weapon _weapon;
        private WeaponWrapper _weaponWrapper;

        [SerializeField] private bool _predictTargetPosition;
        [SerializeField] private LayerMask _attackPredictionMask = 1 << 0 | 1 << 6; // Default Value: Default, Level
        private Vector2 _previousTargetPosition;


        [Space(5)]
        [SerializeField] private float _maxAttackRange = 9f;
        private float _attackCooldownCompleteTime;


        [Header("Keep Distance")]
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;
        public bool ShouldStopAttacking() => Vector2.Distance(_movementScript.transform.position, _targetPos()) > _maxAttackRange;


        [Header("Debug")]
        [SerializeField] private int _attackToDispay;


        public void InitialiseValues(EntityMovement movementScript, Func<Vector2> target)
        {
            this._movementScript = movementScript;
            this._targetPos = target;

            _weaponWrapper = new WeaponWrapper(_weapon, movementScript);
        }


        public override void OnFixedLogic()
        {
            base.OnFixedLogic();

            Vector2 targetPos = _targetPos();

            // Only calculate interception positions and attempt to attack if we are able to attack.
            if (_weaponWrapper.CanAttack())
            {
                // Calculate the position where our target is expected to be when our attack will land.
                Vector2 estimatedTargetPos = targetPos;

                Attack currentAttack = _weaponWrapper.Weapon.Attacks[_weaponWrapper.WeaponAttackIndex];
                Vector2? interceptionPosition = currentAttack.CalculateInterceptionPosition(_movementScript.transform.position, targetPos, (targetPos - _previousTargetPosition) / Time.fixedDeltaTime);
                if (interceptionPosition.HasValue)
                {
                    RaycastHit2D rayHit = Physics2D.Linecast(targetPos, interceptionPosition.Value, _attackPredictionMask);
                    estimatedTargetPos = rayHit.transform != null ? rayHit.point : interceptionPosition.Value;

                    // Debug.
                    Debug.DrawLine(interceptionPosition.Value + Vector2.down, interceptionPosition.Value + Vector2.up, Color.red);
                    Debug.DrawLine(interceptionPosition.Value + Vector2.left, interceptionPosition.Value + Vector2.right, Color.red);
                }


                // Calculate the distance to the target.
                float distanceToTarget = Vector2.Distance(targetPos, _movementScript.transform.position);

                // If we are within range to attack, and our cooldown has elapsed, then make the attack.
                if (distanceToTarget < _maxAttackRange)
                {
                    _weaponWrapper.MakeAttack(estimatedTargetPos, true);
                    //currentAttack.MakeAttack(_movementScript.transform, estimatedTargetPos);
                    _attackCooldownCompleteTime = Time.time + currentAttack.GetRecoveryTime();
                }
            }


            // Cache the target's current position for next frame.
            _previousTargetPosition = targetPos;

            // Move & rotate towards the target with our behaviours set.
            _movementScript.CalculateMovement(targetPos, _movementBehaviours, RotationType.TargetDirection);
        }


        public void DrawGizmos(Transform gizmosOrigin, bool drawBehaviours = false)
        {
            // Draw Max Attack Radius.
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gizmosOrigin.position, _maxAttackRange);


            // Draw Attack Gizmos.
            _weapon.Attacks[_attackToDispay]?.DrawGizmos(gizmosOrigin);


            if (drawBehaviours)
            { 
                // Movement Behaviour Gizmos.
                foreach (BaseSteeringBehaviour behaviour in _movementBehaviours)
                    behaviour.DrawGizmos(gizmosOrigin);
            }
        }
    }
}