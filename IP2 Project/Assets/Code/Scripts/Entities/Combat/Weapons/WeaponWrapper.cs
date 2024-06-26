using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class WeaponWrapper
{
    private MonoBehaviour _linkedScript; // The gameobject this WeaponWrapper is linked to.


    [SerializeField] private Weapon _weapon;
    private int _weaponAttackIndex = 0; // An index for referencing what part of the combo we are in.
    private int _weaponAttackIndexProperty
    {
        get => _weaponAttackIndex;
        set
        {
            // Constraint the value
            if (value >= _weapon.Attacks.Length)
                value = 0;
            else if (value < 0)
                value = _weapon.Attacks.Length - 1;

            // Set the index.
            _weaponAttackIndex = value;
        }
    }

    private Coroutine _resetComboCoroutine;

    private float _attackCompleteTime = 0f; // The time when the current attack will be complete.
    private float _nextReadyTime = 0f; // The time when this weapon can be used again.


    private int _usesRemaining; // How many uses of this weapon are remaining before we need to wait for a recharge.
    private Coroutine _rechargeUsesCoroutine;
    private float _rechargeTimeRemaining = 0f;


    private Coroutine _triggerAttackCoroutine;
    private Coroutine _currentAttackCoroutine;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private int _attackToDebug;


    #region Accessors.
    public Weapon Weapon => _weapon;
    public int WeaponAttackIndex => _weaponAttackIndexProperty;
    public int UsesRemaining => _usesRemaining;
    public float RechargeTimeRemaining => _rechargeTimeRemaining;
    public float RechargePercentage
    {
        get
        {
            // Ensures that we don't get an error from weapons with no recharge time (E.g. Unlimited Uses).
            if (_weapon.TimeToRecharge <= 0)
                return 1f;
            
            // Return the percentage of how far through recharging this weapon is.
            return 1f - (_rechargeTimeRemaining / _weapon.TimeToRecharge);
        }
    }
    public float RecoveryTimePercentage
    {
        get
        {
            // Calculate values.
            float timeTillReady = _nextReadyTime - Time.time;
            int previousAttackIndex = _weaponAttackIndex != 0 ? _weaponAttackIndex - 1 : _weapon.Attacks.Length - 1;

            // If the timeTillReady is less than 0, the recovery time is complete.
            //  Otherwise, calculate the percentage of how far through the recovery we are, based on the previous attack.
            return timeTillReady <= 0
                ? 1f
                : 1f - (timeTillReady / _weapon.Attacks[previousAttackIndex].GetRecoveryTime());
        }
    }
    #endregion



    // Called from Start()/When this WeaponWrapper is created. 
    public WeaponWrapper(Weapon weapon, MonoBehaviour linkedScript)
    {
        // Set the wrapper's weapon.
        this._weapon = weapon;
        // Link the caller to this WeaponWrapper.
        this._linkedScript = linkedScript;

        // Set Initial Values.
        this._weaponAttackIndexProperty = 0;
        this._nextReadyTime = 0f;
        this._usesRemaining = _weapon.UsesBeforeRecharge;
    }

    public bool MakeAttack(Transform attackerTransform, Vector2? targetPos = null, bool throwToTarget = false, System.Action recoveryCompleteAction = null)
    {
        // Ensure this wrapper has been set up.
        if (_linkedScript == null)
        {
            Debug.LogWarning("Warning: WeaponWrapper for " + _weapon.name + " has not been set up before MakeAttack call. Stopping execution");
            return false;
        }

        if (!CanAttack())
            return false;


        // We can attack.
        _triggerAttackCoroutine = _linkedScript.StartCoroutine(TriggerAttack(attackerTransform, targetPos, throwToTarget, recoveryCompleteAction));
        return true;
    }
    public void CancelAttack()
    {
        // Cancel the triggerAttack coroutine.
        if (_triggerAttackCoroutine != null)
            _linkedScript.StopCoroutine(_triggerAttackCoroutine);

        // Cancel the active attack coroutine.
        if (_currentAttackCoroutine != null)
            _linkedScript.StopCoroutine(_currentAttackCoroutine);
    }


    private IEnumerator TriggerAttack(Transform attackerTransform, Vector2? targetPos, bool throwToTarget, System.Action recoveryCompleteAction)
    {
        Attack attack = _weapon.Attacks[_weaponAttackIndexProperty];
        _attackCompleteTime = Time.time + attack.GetWindupTime() + attack.GetDuration();
        _nextReadyTime = Time.time + attack.GetTotalTimeTillNextReady();

        // Stop the reset combo coroutine, if it exists.
        if (_resetComboCoroutine != null)
            _linkedScript.StopCoroutine(_resetComboCoroutine);

        // Apply Kickback Force.
        Vector2 force = -attackerTransform.up * attack.GetKickbackStrength();
        attackerTransform.TryApplyForce(force, attack.GetWindupTime(), ForceMode2D.Impulse);

        // Windup.
        yield return new WaitForSeconds(attack.GetWindupTime());

        // Get references for the attack.
        AttackReferences attackReferences;
        switch (attack)
        {
            case AoEAttack:
                if (targetPos.HasValue && throwToTarget)
                    attackReferences = new AttackReferences(attackerTransform, _linkedScript, targetPos.Value);
                else
                    attackReferences = new AttackReferences(attackerTransform, _linkedScript);

                break;
            default:
                attackReferences = new AttackReferences(attackerTransform, _linkedScript);
                break;
        }

        // Make the attack, caching the returned coroutine for if we should cancel.
        _currentAttackCoroutine = attack.MakeAttack(attackReferences);

        // Recovery.
        yield return new WaitForSeconds(Mathf.Max(attack.GetRecoveryTime() - 0.05f, 0f));

        // Set variables for futher attacks.
        IncrementAttackIndex();
        DecrementWeaponUses();


        yield return new WaitForSeconds(0.05f + Time.deltaTime);

        if (recoveryCompleteAction != null && !Weapon.AllowMovement)
            recoveryCompleteAction?.Invoke();
    }


    public bool IsAttacking() => Time.time < _attackCompleteTime;
    public bool CanAttack()
    {
        // Ensure we can attack pt1 (Ready Time).
        if (Time.time < _nextReadyTime)
            return false;
        // Ensure we can attack pt2 (Are not currently attacking).
        if (IsAttacking())
            return false;
        // Ensure we can attack pt3 (Uses Remaining).
        if (!_weapon.IgnoreUses && _usesRemaining <= 0)
            return false;

        return true;
    }
    private void IncrementAttackIndex()
    {
        // Increment the attack index
        _weaponAttackIndexProperty++;

        // Reset Combo Coroutine.
        if (_resetComboCoroutine != null)
            _linkedScript.StopCoroutine(_resetComboCoroutine);
        _resetComboCoroutine = _linkedScript.StartCoroutine(ResetCombo());
    }
    private void DecrementWeaponUses()
    {
        // Only decrement uses if we aren't ignoring them, AND we are either ticking every use OR are on our first use of the combo.
        if (!_weapon.IgnoreUses && (!_weapon.TickUseOnFirstAttackOnly || _weaponAttackIndex == 0))
        {
            // Decrement Weapon Uses
            _usesRemaining--;

            // Reset weapon uses timer.
            if (!_weapon.RechargeOnlyWhenOut || _usesRemaining <= 0)
            {
                if (_rechargeUsesCoroutine != null)
                    _linkedScript.StopCoroutine(_rechargeUsesCoroutine);
                _rechargeUsesCoroutine = _linkedScript.StartCoroutine(RechargeUses());
            }
        }
    }


    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(_weapon.ComboResetTime);
        _weaponAttackIndex = 0;

    }
    private IEnumerator RechargeUses()
    {
        // Wait TimeToRecharge seconds while still allowing for the accessing of the time left to recharge.
        _rechargeTimeRemaining = _weapon.TimeToRecharge;
        while (_rechargeTimeRemaining > 0)
        {
            _rechargeTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        // Reset the uses.
        _usesRemaining = _weapon.UsesBeforeRecharge;
    }


    public void DrawGizmos(Transform gizmosOrigin)
    {
        if (!_drawGizmos)
            return;

        if (_weapon != null)
        {
            _attackToDebug = Mathf.Clamp(_attackToDebug, 0, _weapon.Attacks.Length - 1);
            _weapon.Attacks[_attackToDebug].DrawGizmos(gizmosOrigin);
        }
    }
}
