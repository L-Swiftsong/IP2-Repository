using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationLinker : MonoBehaviour
{
    [SerializeField] private EntityAnimation _entityAnimation;

    public void OnMovementInput(InputAction.CallbackContext context) => _entityAnimation.PlayMovementAnimation(context.ReadValue<Vector2>());
}
