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
        private WeaponWrapper _weaponWrapperProperty
        {
            get => _weaponWrapper;
            set
            {
                _weaponWrapper = value;
                OnSwappedWeapon?.Invoke(value.Weapon);
            }
        }
        public event Action<Weapon> OnSwappedWeapon;
        [SerializeField] private WeaponAnimator _weaponAnimator;


        [Space(5)]
        [SerializeField] private bool _predictTargetPosition;
        [SerializeField] private LayerMask _attackPredictionMask = 1 << 0 | 1 << 6; // Default Value: Default, Level
        private Vector2 _previousTargetPosition;


        [Space(5)]
        [SerializeField] private float _maxAttackRange = 9f;


        [Header("Keep Distance")]
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;
        public bool ShouldStopAttacking() => Vector2.Distance(_movementScript.transform.position, _targetPos()) > _maxAttackRange;


        [Header("Debug")]
        [SerializeField] private int _attackToDispay;


        public void InitialiseValues(EntityMovement movementScript, Func<Vector2> target)
        {
            this._movementScript = movementScript;
            this._targetPos = target;

            _weaponWrapperProperty = new WeaponWrapper(_weapon, movementScript);
        }


        public override void OnFixedLogic()
        {
            base.OnFixedLogic();

            Vector2 targetPos = _targetPos();


            int previousAttackIndex = _weaponWrapper.WeaponAttackIndex;
            if (AttemptAttack(targetPos))
                _weaponAnimator.StartAttack(0, previousAttackIndex, _weaponWrapper.Weapon.Attacks[previousAttackIndex].GetRecoveryTime());


            // Cache the target's current position for next frame.
            _previousTargetPosition = targetPos;

            // Move & rotate towards the target with our behaviours set.
            _movementScript.CalculateMovement(targetPos, _movementBehaviours, RotationType.TargetDirection);
        }
        private bool AttemptAttack(Vector2 targetPos)
        {
            // Only calculate interception positions and attempt to attack if we are able to attack.
            if (!_weaponWrapperProperty.CanAttack())
                return false;
            

            // Calculate the position where our target is expected to be when our attack will land.
            Vector2 estimatedTargetPos = targetPos;

            Attack currentAttack = _weaponWrapperProperty.Weapon.Attacks[_weaponWrapperProperty.WeaponAttackIndex];
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
                _weaponWrapperProperty.MakeAttack(estimatedTargetPos, true);

            return true;
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