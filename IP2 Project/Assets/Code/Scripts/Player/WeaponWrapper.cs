using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponWrapper
{
    private MonoBehaviour _linkedScript; // The gameobject this WeaponWrapper is linked to.


    [SerializeField] private Weapon _weapon;
    private int _weaponAttackIndex; // An index for referencing what part of the combo we are in.
    private Coroutine _resetComboCoroutine;

    private float _nextReadyTime; // The time when this weapon can be used again.


    private int _usesRemaining; // How many uses of this weapon are remaining before we need to wait for a recharge.
    private Coroutine _rechargeUsesCoroutine;


    // Accessors.
    public Weapon Weapon => _weapon;
    public int WeaponAttackIndex => _weaponAttackIndex;
    public int UsesRemaining => _usesRemaining;


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

    public void MakeAttack(Vector2? targetPos = null, bool throwToMouse = false)
    {
        // Ensure this wrapper has been set up.
        if (_linkedScript == null)
        {
            Debug.LogWarning("Warning: WeaponWrapper for " + _weapon.name + " has not been set up before MakeAttack call. Stopping execution");
            return;
        }

        if (!CanAttack())
            return;

        // We can attack.
        // Make the attack.
        Attack attack = _weapon.Attacks[_weaponAttackIndex];
        switch (attack)
        {
            case AoEAttack:
                if (targetPos.HasValue && throwToMouse)
                    attack.MakeAttack(_linkedScript.transform, targetPos.Value);
                else
                    attack.MakeAttack(_linkedScript.transform);

                break;
            default:
                attack.MakeAttack(_linkedScript.transform);
                break;
        }

        // Set variables for futher tasks.
        _nextReadyTime = Time.time + attack.GetRecoveryTime();

        IncrementAttackIndex();
        DecrementWeaponUses();
    }


    private bool CanAttack()
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
            if (_rechargeUsesCoroutine != null)
                _linkedScript.StopCoroutine(_rechargeUsesCoroutine);
            _rechargeUsesCoroutine = _linkedScript.StartCoroutine(RechargeUses());
        }
    }


    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(_weapon.ComboResetTime);
        _weaponAttackIndex = 0;
    }
    private IEnumerator RechargeUses()
    {
        yield return new WaitForSeconds(_weapon.TimeToRecharge);
        _usesRemaining = _weapon.UsesBeforeRecharge;
    }
}