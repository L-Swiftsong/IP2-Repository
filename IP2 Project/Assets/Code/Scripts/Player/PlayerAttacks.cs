using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerAttacks : MonoBehaviour
{
    [Header("Primary Attacks")]
    [SerializeField] private WeaponWrapper _primaryWeapon;
    public bool _primaryAttackHeld;

    [Header("Secondary Attacks")]
    [SerializeField] private WeaponWrapper _secondaryWeapon;
    public bool _secondaryAttackHeld;

    [SerializeField, Min(0)] private int _attackToDebug;


    [Header("Abilities")]
    [SerializeField] private Ability _currentAbility;
    private float _abilityCooldownComplete;
    private bool _useAbilityHeld;


    [Header("AoE Test")]
    [SerializeField] private Camera _playerCam;
    [SerializeField] private bool _throwToMouse;
    private Vector2 _mousePosition;

    [Header("Cooldown for ability")]
    [SerializeField] Image Coolbar;
    [SerializeField] float energy, maxEnergy;
    [SerializeField] float attackCost;
    [SerializeField] float chargeRate;
    private Coroutine recharge;


    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        /*if (context.started && energy == maxEnergy)
        {
            _secondaryAttackHeld = true;
            energy -= attackCost;
            Coolbar.fillAmount = energy / maxEnergy;

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeAttack());
        }


        else if (context.canceled && energy != maxEnergy)
            _secondaryAttackHeld = false;*/
        if (context.started)
            _secondaryAttackHeld = true;
        else if (context.canceled)
            _secondaryAttackHeld = false;
    }
    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            _primaryAttackHeld = true;
        else if (context.canceled)
            _primaryAttackHeld = false;
    }

    
    public void OnUseAbilityPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            _useAbilityHeld = true;
        else if (context.canceled)
            _useAbilityHeld = false;
    }
    public void GetMousePosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 worldPos = _playerCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            _mousePosition = worldPos;
        }
    }


    private void Start()
    {
        if (_playerCam == null)
            _playerCam = Camera.main;
        
        _primaryWeapon = new WeaponWrapper(_primaryWeapon.Weapon, this);
        _secondaryWeapon = new WeaponWrapper(_secondaryWeapon.Weapon, this);
    }
    private void Update()
    {
        // Abilities are checked before attacks.
        if (_useAbilityHeld && CanUseAbility())
        {
            UseAbility();
        }
        else if (_primaryAttackHeld)
        {
            AttemptAttack(_primaryWeapon);
        }

        else if (_secondaryAttackHeld)
        {
            AttemptAttack(_secondaryWeapon);
        }

        Coolbar.fillAmount = _secondaryWeapon.RechargePercentage;
    }

    private void AttemptAttack(WeaponWrapper weapon) => weapon.MakeAttack(_mousePosition, throwToMouse: _throwToMouse);


    public void EquipWeapon(Weapon newWeapon, bool replacePrimary = true)
    {
        WeaponWrapper newWrapper = new WeaponWrapper(newWeapon, this);

        if (replacePrimary)
            _primaryWeapon = newWrapper;
        else
            _secondaryWeapon = newWrapper;
    }


    private bool CanUseAbility()
    {
        if (_abilityCooldownComplete >= Time.time)
            return false;

        return true;
    }
    private void UseAbility()
    {
        Debug.Log("Used Ability");
        _abilityCooldownComplete = Time.time + _currentAbility.GetCooldownTime();
    }



    private void OnDrawGizmos()
    {
        if (_primaryWeapon.Weapon != null && _attackToDebug < _primaryWeapon.Weapon.Attacks.Length)
            _primaryWeapon.Weapon.Attacks[_attackToDebug].DrawGizmos(this.transform);

        else if (_secondaryWeapon.Weapon != null && _attackToDebug < _secondaryWeapon.Weapon.Attacks.Length)
            _secondaryWeapon.Weapon.Attacks[_attackToDebug].DrawGizmos(this.transform);
    }

    private IEnumerator RechargeAttack()
    {
        yield return new WaitForSeconds(1f);

        while (energy < maxEnergy)
        {
            energy += chargeRate / 10f;

            if (energy > maxEnergy)
            {
                energy = maxEnergy;
            }

            Coolbar.fillAmount = energy / maxEnergy;

            yield return new WaitForSeconds(.1f);


        }
    }
}
