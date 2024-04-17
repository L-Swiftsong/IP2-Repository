using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationLinker : MonoBehaviour
{
    [SerializeField] private EntityAnimator _entityAnimation;
    private Vector2 _movementInput;

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
            _movementInput = Vector2.zero;
        else
            _movementInput = context.ReadValue<Vector2>();
    }

    private void Update() => _entityAnimation.PlayMovementAnimation(_movementInput);
}