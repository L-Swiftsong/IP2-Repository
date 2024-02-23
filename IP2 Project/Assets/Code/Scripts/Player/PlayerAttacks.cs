using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerAttacks : MonoBehaviour
{
    [Header("Primary Attacks")]
    [SerializeField] private WeaponWrapper _primaryWeapon;
    private WeaponWrapper _primaryWeaponProperty
    {
        get => _primaryWeapon;
        set
        {
            _primaryWeapon = value;
            OnPrimaryWeaponChanged?.Invoke(value.Weapon);
        }
    }
    private bool _primaryAttackHeld;
    
    public static System.Action<float> OnPrimaryRecoveryTimeChanged; // Called every frame while the weapon's RechargePercentage is below 1.
    public static System.Action<float> OnPrimaryUseRechargeTimeChanged; // Called every frame while the weapon is recharging its uses.
    public static System.Action<Weapon> OnPrimaryWeaponChanged; // Called when the primaryWeapon is assigned.


    [Header("Secondary Attacks")]
    [SerializeField] private WeaponWrapper _secondaryWeapon;
    private WeaponWrapper _secondaryWeaponProperty
    {
        get => _secondaryWeapon;
        set
        {
            _secondaryWeapon = value;
            OnSecondaryWeaponChanged?.Invoke(value.Weapon);
        }
    }
    private bool _secondaryAttackHeld;

    public static System.Action<float> OnSecondaryRecoveryTimeChanged; // Called every frame while the weapon's RechargePercentage is below 1.
    public static System.Action<float> OnSecondaryUseRechargeTimeChanged; // Called every frame while the weapon is recharging its uses.
    public static System.Action<Weapon> OnSecondaryWeaponChanged; // Called when the primaryWeapon is assigned.


    [Header("AoE Test")]
    [SerializeField] private Camera _playerCam;
    [SerializeField] private bool _throwToMouse;
    private Vector2 _mousePosition;


    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
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

        _primaryWeaponProperty = new WeaponWrapper(_primaryWeaponProperty.Weapon, this);
        _secondaryWeaponProperty = new WeaponWrapper(_secondaryWeaponProperty.Weapon, this);
    }
    private void Update()
    {
        // Check if the primary attack button is held.
        if (_primaryAttackHeld)
        {
            AttemptAttack(_primaryWeaponProperty);
        }
        
        // Check if the secondary attack button is held.
        if (_secondaryAttackHeld)
        {
            AttemptAttack(_secondaryWeapon);
        }


        // Call Events for UI (Currently occuring every frame. We can optimise this by reducing the number of calls).
        // (Primary Weapon)
        OnPrimaryRecoveryTimeChanged?.Invoke(_primaryWeaponProperty.RecoveryTimePercentage); // Recovery Event (Time between attacks).
        OnPrimaryUseRechargeTimeChanged?.Invoke(_primaryWeaponProperty.RechargePercentage); // Recharge Event (Uses Remaining).

        // (Secondary Weapon)
        OnSecondaryRecoveryTimeChanged?.Invoke(_secondaryWeaponProperty.RecoveryTimePercentage); // Recovery Event (Time between attacks).
        OnSecondaryUseRechargeTimeChanged?.Invoke(_secondaryWeaponProperty.RechargePercentage); // Recharge Event (Uses Remaining).
    }

    private void AttemptAttack(WeaponWrapper weapon) => weapon.MakeAttack(_mousePosition, throwToMouse: _throwToMouse);


    public void EquipWeapon(Weapon newWeapon, bool replacePrimary = true)
    {
        WeaponWrapper newWrapper = new WeaponWrapper(newWeapon, this);

        if (replacePrimary)
            _primaryWeaponProperty = newWrapper;
        else
            _secondaryWeaponProperty = newWrapper;
    }



    //private void OnDrawGizmos()
    //{
    //    if (_primaryWeapon.Weapon != null && _attackToDebug < _primaryWeapon.Weapon.Attacks.Length)
    //        _primaryWeapon.Weapon.Attacks[_attackToDebug].DrawGizmos(this.transform);

    //    else if (_secondaryWeapon.Weapon != null && _attackToDebug < _secondaryWeapon.Weapon.Attacks.Length)
    //        _secondaryWeapon.Weapon.Attacks[_attackToDebug].DrawGizmos(this.transform);
    //}
}
