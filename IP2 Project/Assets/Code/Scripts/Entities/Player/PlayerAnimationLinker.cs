using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationLinker : MonoBehaviour
{
    [SerializeField] private EntityAnimator _entityAnimation;

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _entityAnimation.PlayMovementAnimation(input);
        _entityAnimation.SetMovementInputValue(input);
    }
}
