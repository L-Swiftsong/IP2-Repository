using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PlayerAttacks : MonoBehaviour
{
    [SerializeField] private Transform _rotationPivot;


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
    [SerializeField] private bool _throwToMouse;
    private Camera _playerCam;
    private Vector2 _mousePosition;


    [Header("Animations")]
    public UnityEvent<WeaponAnimationValues> OnAttackStarted;


    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            _primaryAttackHeld = true;
        else if (context.canceled)
            _primaryAttackHeld = false;

    }

    

    
    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            _secondaryAttackHeld = true;
        else if (context.canceled)
            _secondaryAttackHeld = false;

       

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
        _playerCam = GameManager.MainCamera;

        _primaryWeaponProperty = new WeaponWrapper(_primaryWeaponProperty.Weapon, this);
        _secondaryWeaponProperty = new WeaponWrapper(_secondaryWeaponProperty.Weapon, this);


     

    }
    private void Update()
    {
        // Check if the secondary attack button is held (Medium Priority).
        if (_secondaryAttackHeld)
        {
            AttemptAttack(_secondaryWeapon);
        }
	    // Check if the primary attack button is held (Lowest Priority).
	    else if (_primaryAttackHeld)
	    {
            AttemptAttack(_primaryWeaponProperty);
	    }


        // Call Events for UI (Currently occuring every frame. We can optimise this by reducing the number of calls).
        // (Primary Weapon)
        OnPrimaryRecoveryTimeChanged?.Invoke(_primaryWeaponProperty.RecoveryTimePercentage); // Recovery Event (Time between attacks).
        OnPrimaryUseRechargeTimeChanged?.Invoke(_primaryWeaponProperty.RechargePercentage); // Recharge Event (Uses Remaining).

        // (Secondary Weapon)
        OnSecondaryRecoveryTimeChanged?.Invoke(_secondaryWeaponProperty.RecoveryTimePercentage); // Recovery Event (Time between attacks).
        OnSecondaryUseRechargeTimeChanged?.Invoke(_secondaryWeaponProperty.RechargePercentage); // Recharge Event (Uses Remaining).

       
    }

    private void AttemptAttack(WeaponWrapper weapon)
    {
        int previousAttackIndex = _primaryWeaponProperty.WeaponAttackIndex;
        if (weapon.MakeAttack(_rotationPivot, _mousePosition, throwToTarget: _throwToMouse))
        {
            OnAttackStarted?.Invoke(new WeaponAnimationValues(_primaryWeapon == weapon ? 0 : 1, previousAttackIndex, _primaryWeaponProperty.Weapon.Attacks[previousAttackIndex].GetTotalAttackTime()));
        }

    }

    public void EquipWeapon(Weapon newWeapon, bool replacePrimary = true)
    {
        WeaponWrapper newWrapper = new WeaponWrapper(newWeapon, this);

        if (replacePrimary)
            _primaryWeaponProperty = newWrapper;
        else
            _secondaryWeaponProperty = newWrapper;
    }


    //private bool CanUseAbility() => Time.time >= _abilityCooldownTime;
    //private void UseAbility()
    //{
      //  Debug.Log("Used Ability: " + _currentAbility.name);
      //  _abilityCooldownTime = Time.time + _currentAbility.GetCooldownTime();
    //}


    // Remove and replace with the version on the 'Animations' branch.
    private void OnDrawGizmos()
    {
        if (_primaryWeaponProperty != null)
            _primaryWeaponProperty.DrawGizmos(_rotationPivot);
        if (_secondaryWeaponProperty != null)
            _secondaryWeaponProperty.DrawGizmos(_rotationPivot);
    }
}
