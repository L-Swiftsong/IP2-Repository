using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponWrapper
{
    private MonoBehaviour _linkedScript; // The gameobject this WeaponWrapper is linked to.


    [SerializeField] private Weapon _weapon;
    private int _weaponAttackIndex = 0; // An index for referencing what part of the combo we are in.
    private Coroutine _resetComboCoroutine;

    private float _nextReadyTime = 0f; // The time when this weapon can be used again.


    private int _usesRemaining; // How many uses of this weapon are remaining before we need to wait for a recharge.
    private Coroutine _rechargeUsesCoroutine;
    private float _rechargeTimeRemaining = 0f;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private int _attackToDebug;


    #region Accessors.
    public Weapon Weapon => _weapon;
    public int WeaponAttackIndex => _weaponAttackIndex;
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
        this._weaponAttackIndex = 0;
        this._nextReadyTime = 0f;
        this._usesRemaining = _weapon.UsesBeforeRecharge;
    }

    public bool MakeAttack(Vector2? targetPos = null, bool throwToTarget = false)
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
        _linkedScript.StartCoroutine(TriggerAttack(targetPos, throwToTarget));
        return true;
    }
    private IEnumerator TriggerAttack(Vector2? targetPos, bool throwToTarget)
    {
        Attack attack = _weapon.Attacks[_weaponAttackIndex];
        _nextReadyTime = Time.time + attack.GetTotalAttackTime();
        Debug.Log("Start Attack");

        // Windup.
        yield return new WaitForSeconds(attack.GetWindupTime());
        Debug.Log("Make Attack");
        
        // Make the attack.
        switch (attack)
        {
            case AoEAttack:
                if (targetPos.HasValue && throwToTarget)
                    attack.MakeAttack(_linkedScript.transform, targetPos.Value);
                else
                    attack.MakeAttack(_linkedScript.transform);

                break;
            default:
                attack.MakeAttack(_linkedScript.transform);
                break;
        }

        // Set variables for futher tasks.
        IncrementAttackIndex();
        DecrementWeaponUses();
    }


    public bool CanAttack()
    {
        // Ensure we can attack pt1 (Ready Time).
        if (Time.time < _nextReadyTime)
            return false;
        // Ensure we can attack pt2 (Uses Remaining).
        if (!_weapon.IgnoreUses && _usesRemaining <= 0)
            return false;

        return true;
    }
    private void IncrementAttackIndex()
    {
        // Increment the attack index
        if (_weaponAttackIndex < _weapon.Attacks.Length - 1)
            _weaponAttackIndex++;
        else
            _weaponAttackIndex = 0;

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
