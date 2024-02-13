using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttacks : MonoBehaviour
{
    [Header("Primary Attacks")]
    [SerializeField] private WeaponWrapper _primaryWeapon;
    private bool _primaryAttackHeld;

    [SerializeField, Min(0)] private int _attackToDebug;


    [Header("Abilities")]
    [SerializeField] private Ability _currentAbility;
    private float _abilityCooldownComplete;
    private bool _useAbilityHeld;


    [Header("AoE Test")]
    [SerializeField] private Camera _playerCam;
    [SerializeField] private bool _throwToMouse;
    private Vector2 _mousePosition;


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


    private void Start() => _primaryWeapon = new WeaponWrapper(_primaryWeapon.Weapon, this);

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
    }

    private void AttemptAttack(WeaponWrapper weapon) => weapon.MakeAttack(_mousePosition, throwToMouse: _throwToMouse);


    public void EquipWeapon(Weapon newWeapon, bool replacePrimary = true)
    {
        WeaponWrapper newWrapper = new WeaponWrapper(newWeapon, this);

        if (replacePrimary)
            _primaryWeapon = newWrapper;
        // else
        //  _secondaryWeapon = newWrapper;
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
    }
}
