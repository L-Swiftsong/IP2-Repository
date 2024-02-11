using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttacks : MonoBehaviour
{
    [Header("Primary Attacks")]
    [SerializeField] private Attack[] _primaryAttacks;
    private int _primaryAttackIndex;
    private bool _primaryAttackHeld;

    private float _nextAttackAvailableTime;

    [SerializeField, Min(0)] private int _attackToDebug;


    [Space(5)]
    [SerializeField] private float _resetPrimaryComboDuration;
    private Coroutine _resetPrimaryComboCoroutine;


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


    private void Update()
    {
        // Abilities are checked before attacks.
        if (_useAbilityHeld && CanUseAbility())
        {
            UseAbility();
        }
        else if (_primaryAttackHeld && CanAttack())
        {
            AttemptAttack(_primaryAttacks[_primaryAttackIndex]);
        }
    }


    private bool CanAttack()
    {
        // If we cannot attack yet, return false.
        if (_nextAttackAvailableTime >= Time.time)
            return false;

        // Otherwise, return true.
        return true;
    }
    private void AttemptAttack(Attack attack)
    {
        // Make the attack.
        switch (attack)
        {
            case AoEAttack:
                if (_throwToMouse)
                    attack.MakeAttack(this.transform, _mousePosition);
                else
                    attack.MakeAttack(this.transform);

                break;
            default:
                attack.MakeAttack(this.transform);
                break;
        }

        // Update the recovery time.
        _nextAttackAvailableTime = Time.time + attack.GetRecoveryTime();


        IncrementPrimaryCombo();
    }

    private void IncrementPrimaryCombo()
    {
        // Update Primary Attack Combo Index.
        if (_primaryAttackIndex < _primaryAttacks.Length - 1)
            _primaryAttackIndex++;
        else
            _primaryAttackIndex = 0;

        // Start reset combo coroutine.
        if (_resetPrimaryComboCoroutine != null)
            StopCoroutine(_resetPrimaryComboCoroutine);
        _resetPrimaryComboCoroutine = StartCoroutine(ResetPrimaryCombo());
    }
    private IEnumerator ResetPrimaryCombo()
    {
        yield return new WaitUntil(() => CanAttack());
        yield return new WaitForSeconds(_resetPrimaryComboDuration);

        Debug.Log("Combo Reset");
        _primaryAttackIndex = 0;
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
        if (_attackToDebug < _primaryAttacks.Length)
            _primaryAttacks[_attackToDebug].DrawGizmos(this.transform);
    }
}
