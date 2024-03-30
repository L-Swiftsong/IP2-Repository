using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


namespace States.Alternative
{
    [System.Serializable]
    public class ComplexAttacking : State
    {
        public override string Name { get => "Attacking"; }


        private MonoBehaviour _monoScript;
        private Transform _rotationPivot;
        private Func<Vector2> _targetPos;
        private EntityMovement _movementScript;


        [Header("Attacks")]
        [SerializeField] private Weapon[] _weapons;
        private WeaponWrapper[] _weaponWrappers;


        [Space(5)]
        [SerializeField] private bool _simultaniousWeaponUsage = false;

        [Space(5)]
        [SerializeField] private float _weaponSwitchTickRate = 1f;
        [SerializeField, Range(0f, 1f)] private float _weaponSwitchChance = 0.5f;
        private int _currentWeaponIndex;
        private Coroutine _swapWeaponsCoroutine;


        [Header("Attacking Paramters")]
        [SerializeField] private bool _predictTargetPosition;
        [SerializeField] private LayerMask _attackPredictionMask = 1 << 0 | 1 << 6; // Default Value: Default, Level
        private Vector2 _previousTargetPosition;


        [Space(5)]
        [SerializeField] private float _maxAttackRange = 9f;


        [Header("Keep Distance")]
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;
        public bool ShouldStopAttacking() => Vector2.Distance(_movementScript.transform.position, _targetPos()) > _maxAttackRange;


        [Header("Animation")]
        public UnityEngine.Events.UnityEvent<WeaponAnimationValues> OnAttackStarted; // Should subscribe to WeaponAnimator.StartAttack & EntityAnimation.PlayAttackAnimation.
        public UnityEngine.Events.UnityEvent<Weapon, int> OnWeaponChanged; // Should Subscribe to WeaponAnimator.OnWeaponChanged.


        [Header("Debug")]
        [SerializeField] private int _weaponToDisplay;
        [SerializeField] private int _attackToDispay;


        public void InitialiseValues(MonoBehaviour monoScript, Transform rotationPivot, EntityMovement movementScript, Func<Vector2> target)
        {
            this._monoScript = monoScript;
            this._rotationPivot = rotationPivot;
            this._movementScript = movementScript;
            this._targetPos = target;

            // Setup the Weapon Wrappers.
            _weaponWrappers = new WeaponWrapper[_weapons.Length];
            for (int i = 0; i < _weapons.Length; i++)
            {
                SetWeaponWrapper(_weapons[i], i);
            }
        }


        public override void OnEnter()
        {
            base.OnEnter();


            if (_swapWeaponsCoroutine != null)
                _movementScript.StopCoroutine(_swapWeaponsCoroutine);

            if (!_simultaniousWeaponUsage)
                _movementScript.StartCoroutine(SwapWeapons());
        }
        public override void OnFixedLogic()
        {
            base.OnFixedLogic();

            Vector2 targetPos = _targetPos();


            if (_simultaniousWeaponUsage)
            {
                for (int i = 0; i < _weapons.Length; i++)
                {
                    int previousAttackIndex = _weaponWrappers[i].WeaponAttackIndex;
                    if (AttemptAttack(_weaponWrappers[i], targetPos))
                    {
                        WeaponAnimationValues animationValues = new WeaponAnimationValues(i, previousAttackIndex, _weaponWrappers[i].Weapon.Attacks[previousAttackIndex].GetTotalAttackTime());
                        OnAttackStarted?.Invoke(animationValues);
                    }
                }
            }
            else
            {
                int previousAttackIndex = _weaponWrappers[_currentWeaponIndex].WeaponAttackIndex;
                if (AttemptAttack(_weaponWrappers[_currentWeaponIndex], targetPos))
                {
                    WeaponAnimationValues animationValues = new WeaponAnimationValues(_currentWeaponIndex, previousAttackIndex, _weaponWrappers[_currentWeaponIndex].Weapon.Attacks[previousAttackIndex].GetTotalAttackTime());
                    OnAttackStarted?.Invoke(animationValues);
                }
            }

            // Cache the target's current position for next frame.
            _previousTargetPosition = targetPos;

            // Move & rotate towards the target with our behaviours set.
            _movementScript.CalculateMovement(targetPos, _movementBehaviours, RotationType.TargetDirection);
        }

        private bool AttemptAttack(WeaponWrapper weaponWrapper, Vector2 targetPos)
        {
            // Only continue if we are able to attack.
            if (!weaponWrapper.CanAttack())
                return false;
            

            // Calculate the position where our target is expected to be when our attack will land.
            Vector2 estimatedTargetPos = targetPos;

            Attack currentAttack = weaponWrapper.Weapon.Attacks[weaponWrapper.WeaponAttackIndex];
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
                weaponWrapper.MakeAttack(_rotationPivot, _monoScript, estimatedTargetPos, true);
            }


            return true;
        }
        private IEnumerator SwapWeapons()
        {
            float randomChance;
            while(!_simultaniousWeaponUsage && _weaponWrappers.Length > 1)
            {
                // Roll to see if we should swap the current weapon.
                randomChance = UnityEngine.Random.Range(0f, 1f);
                if (_weaponSwitchChance > randomChance)
                {
                    // Calculate the new value for the weapon index, ensuring it is not the same as the current.
                    int randomValue = UnityEngine.Random.Range(0, _weaponWrappers.Length - 1);
                    _currentWeaponIndex = randomValue < _currentWeaponIndex ? randomValue : randomValue + 1;
                }

                yield return new WaitForSeconds(_weaponSwitchTickRate);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            // Stop the swap weapons coroutine.
            if (_swapWeaponsCoroutine != null)
                _movementScript.StopCoroutine(_swapWeaponsCoroutine);
        }


        private void SetWeaponWrapper(Weapon newWeapon, int index)
        {
            _weaponWrappers[index] = new WeaponWrapper(newWeapon, _movementScript);
            OnWeaponChanged?.Invoke(newWeapon, index);
        }


        public void DrawGizmos(Transform gizmosOrigin, bool drawBehaviours = false)
        {
            // Draw Max Attack Radius.
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gizmosOrigin.position, _maxAttackRange);


            // Draw Attack Gizmos.
            _weapons[_weaponToDisplay].Attacks[_attackToDispay]?.DrawGizmos(gizmosOrigin);


            if (drawBehaviours)
            { 
                // Movement Behaviour Gizmos.
                foreach (BaseSteeringBehaviour behaviour in _movementBehaviours)
                    behaviour.DrawGizmos(gizmosOrigin);
            }
        }
    }
}