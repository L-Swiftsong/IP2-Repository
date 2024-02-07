using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttacks : MonoBehaviour
{
    [SerializeField] private Attack[] _primaryAttacks;
    private int _primaryAttackIndex;

    [Space(5)]
    [SerializeField] private float _resetPrimaryComboDuration;
    private Coroutine _resetPrimaryComboCoroutine;
    

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttemptAttack();
    }


    private void AttemptAttack()
    {
        Debug.Log("Primary Attack: " + _primaryAttackIndex);


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

        Debug.Log("Reset Combo");
        _primaryAttackIndex = 0;
    }
}
