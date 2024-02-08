using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttacks : MonoBehaviour
{
    [Header("Primary Attacks")]
    [SerializeField] private Attack[] _primaryAttacks;
    private int _primaryAttackIndex;

    [SerializeField, Min(0)] private int _attackToDebug;


    [Space(5)]
    [SerializeField] private float _resetPrimaryComboDuration;
    private Coroutine _resetPrimaryComboCoroutine;


    [Header("Abilities")]
    [SerializeField] private bool _temp;


    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttemptAttack();
    }
    public void OnUseAbilityPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            UseAbility();
    }


    private void AttemptAttack()
    {
        Debug.Log("Primary Attack: " + _primaryAttackIndex);
        _primaryAttacks[_primaryAttackIndex].MakeAttack(this.transform);


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
        yield return new WaitForSeconds(_resetPrimaryComboDuration);

        Debug.Log("Combo Reset");
        _primaryAttackIndex = 0;
    }

    private void UseAbility()
    {
        Debug.Log("Used Ability");
    }



    private void OnDrawGizmos()
    {
        if (_attackToDebug < _primaryAttacks.Length)
            _primaryAttacks[_attackToDebug].DrawGizmos(this.transform);
    }
}
