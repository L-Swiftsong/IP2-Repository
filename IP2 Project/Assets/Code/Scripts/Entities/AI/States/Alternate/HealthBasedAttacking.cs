using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


namespace States.Alternative
{
    [System.Serializable]
    public class HealthBasedAttacking : State
    {
        public override string Name { get => "Attacking"; }


        private MonoBehaviour _monoScript;
        private Transform _rotationPivot;
        private Func<Vector2> _targetPos;
        private EntityMovement _movementScript;
        private HealthComponent _healthScript;


        [System.Serializable]
        private struct WeaponThreshold
        {
            public string Name;
            public Weapon Weapon;
            [HideInInspector] public WeaponWrapper WeaponWrapper;


            [Header("Targeting")]
            public bool PredictTargetPosition;
            public float MaxRange;


            [Header("Health Thresholds")]
            [Range(0, 1)] public float MaxHealthPercentage; // Max - Inclusive.
            [Range(0, 1)] public float MinHealthPercentage; // Min - Exclusive.


            [Space(5)]
            [ReadOnly] public bool IsActive;

            public void Init(MonoBehaviour linkedScript) => this.WeaponWrapper = new WeaponWrapper(this.Weapon, linkedScript);
            public void EnableIfShouldBeActive(float currentHealthPercentage) => IsActive = (currentHealthPercentage > MinHealthPercentage) && (currentHealthPercentage <= MaxHealthPercentage);
        }



        [Header("Attacks")]
        [SerializeField] private WeaponThreshold[] _weapons;
        private bool _canAttack;


        [Header("Attacking Paramters")]
        [SerializeField] private LayerMask _attackPredictionMask = 1 << 0 | 1 << 6; // Default Value: Default, Level
        private Vector2 _previousTargetPosition;


        [Header("Keep Distance")]
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;


        [Header("Animation")]
        [Tooltip("Called when an attack is started. By default should subscribe to WeaponAnimator.StartAttack & EntityAnimation.PlayAttackAnimation")]
            public UnityEngine.Events.UnityEvent<WeaponAnimationValues> OnAttackStarted;
        [Tooltip("Called when the state is exited. By default should subscribe to WeaponAnimator.CancelAttack")]
            public UnityEngine.Events.UnityEvent OnAttackCancelled;
        [Tooltip("Called when a weapon is changed. By default should Subscribe to WeaponAnimator.OnWeaponChanged")]
            public UnityEngine.Events.UnityEvent<Weapon, int> OnWeaponChanged;


        [Header("Debug")]
        [SerializeField, ReadOnly] private string _currentWeapon;



        public void InitialiseValues(MonoBehaviour parentScript, Transform rotationPivot, EntityMovement movementScript, HealthComponent healthScript, Func<Vector2> target)
        {
            this._monoScript = parentScript;
            this._rotationPivot = rotationPivot;
            this._movementScript = movementScript;
            this._healthScript = healthScript;
            this._targetPos = target;

            
            // Setup the Weapon Wrappers.
            for (int i = 0; i < _weapons.Length; i++)
                SetupWeaponWrapper(i, parentScript);


            // Enable weapons after 1 frame (Prevents issues with HealthComponent's Start calling after this Init & healthPercentage being 0).
            _monoScript.StartCoroutine(WaitOneFrameThenTryEnableWeapons());
        }
        private void SetupWeaponWrapper(int index, MonoBehaviour linkedScript)
        {
            // Setup the WeaponWrapper.
            _weapons[index].Init(linkedScript);

            // Notify the WeaponAnimator of the weapon's existence.
            OnWeaponChanged?.Invoke(_weapons[index].WeaponWrapper.Weapon, index);
        }
        private IEnumerator WaitOneFrameThenTryEnableWeapons()
        {
            yield return null;

            // Check what weapons should start enabled.
            float healthPercentage = _healthScript.GetHealthPercentage();
            for (int i = 0; i < _weapons.Length; i++)
                _weapons[i].EnableIfShouldBeActive(healthPercentage);
        }


        public override void OnEnter()
        {
            base.OnEnter();

            // Subscribe to the HealthComponent.
            _healthScript.OnHealingReceived.AddListener(OnHealthChanged);
            _healthScript.OnDamageTaken.AddListener(OnHealthChanged);
        }
        public override void OnFixedLogic()
        {
            base.OnFixedLogic();

            // Cache the targetPos.
            Vector2 targetPos = _targetPos();

            // Determine if we can attack.
            _canAttack = _weapons.Where(t => t.IsActive).All(t => t.WeaponWrapper.IsAttacking() == false);

            if (_canAttack)
            {
                // Attempt To Make an Attack (Only 1 attack per frame).
                for (int i = 0; i < _weapons.Length; i++)
                {
                    // Attempt the attack (Notifying the WeaponAnimator if we were successful.
                    int previousAttackIndex = _weapons[i].WeaponWrapper.WeaponAttackIndex;
                    if (AttemptAttack(_weapons[i], targetPos))
                    {
                        WeaponAnimationValues animationValues = new WeaponAnimationValues(i, previousAttackIndex, _weapons[i].WeaponWrapper.Weapon.Attacks[previousAttackIndex].GetTotalAttackTime());
                        OnAttackStarted?.Invoke(animationValues);
                        break;
                    }
                }
            }

            // Cache the target's current position for next frame.
            _previousTargetPosition = targetPos;

            // Move & rotate towards the target with our behaviours set.
            _movementScript.CalculateMovement(targetPos, _movementBehaviours, RotationType.TargetDirection);
        }
        public override void OnExit()
        {
            base.OnExit();

            // Unsubscribe from HealthComponent.
            _healthScript.OnHealingReceived.RemoveListener(OnHealthChanged);
            _healthScript.OnDamageTaken.RemoveListener(OnHealthChanged);

            // Cancel the current attack.
            OnAttackCancelled?.Invoke();
            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i].WeaponWrapper.CancelAttack();
            }
        }
        // Only allow exits if NONE of the weaponWrappers are currently attacking.
        protected override bool CanExit() => _weapons.Any(t => t.WeaponWrapper.CanAttack()) == false;



        private bool AttemptAttack(WeaponThreshold weaponThreshold, Vector2 targetPos)
        {
            // Cache the WeaponWrapper.
            WeaponWrapper weaponWrapper = weaponThreshold.WeaponWrapper;

            // Only continue if we are able to attack.
            if (!weaponThreshold.IsActive || !weaponThreshold.WeaponWrapper.CanAttack())
                return false;


            // Calculate the position where our target is expected to be when our attack will land.
            Vector2 estimatedTargetPos = targetPos;

            Attack currentAttack = weaponWrapper.Weapon.Attacks[weaponWrapper.WeaponAttackIndex];
            if (weaponThreshold.PredictTargetPosition)
            {
                // If this weapon allows us to predict the target's position, then do so.
                Vector2? interceptionPosition = currentAttack.CalculateInterceptionPosition(_movementScript.transform.position, targetPos, (targetPos - _previousTargetPosition) / Time.fixedDeltaTime);
                if (interceptionPosition.HasValue)
                {
                    // Ensure that we do not estimate our target walking through walls.
                    RaycastHit2D rayHit = Physics2D.Linecast(targetPos, interceptionPosition.Value, _attackPredictionMask);
                    estimatedTargetPos = rayHit.transform != null ? rayHit.point : interceptionPosition.Value;

                    // Debug Cross.
                    Debug.DrawLine(interceptionPosition.Value + Vector2.down, interceptionPosition.Value + Vector2.up, Color.red);
                    Debug.DrawLine(interceptionPosition.Value + Vector2.left, interceptionPosition.Value + Vector2.right, Color.red);
                }
            }


            // Calculate the distance to the target.
            float distanceToTarget = Vector2.Distance(targetPos, _movementScript.transform.position);

            // If we are within range to attack, and our cooldown has elapsed, then make the attack.
            if (distanceToTarget < weaponThreshold.MaxRange)
            {
                if (weaponWrapper.MakeAttack(_rotationPivot, _monoScript, estimatedTargetPos, true))
                    return true;
            }
            
            // Either we were too far to attack, or the weaponWrapper's MakeAttack() failed, so return false to show that we didn't attack.
            return false;
        }


        private void OnHealthChanged(HealthChangedValues newValues)
        {
            // Calculate the new health percentage.
            float healthPercentage = (float)newValues.NewHealth / (float)newValues.NewMax;
            Debug.Log("Health Percentage: " + healthPercentage);

            // Enable weapon that can now attack & disable those that cannot.
            for (int i = 0; i < _weapons.Length; i++)
                _weapons[i].EnableIfShouldBeActive(healthPercentage);
        }
    }
}