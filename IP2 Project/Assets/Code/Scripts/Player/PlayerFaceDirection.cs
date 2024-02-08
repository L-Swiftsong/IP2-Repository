using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFaceDirection : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    private const string MOUSE_AND_KEYBOARD_SCHEME_NAME = "MnK";
    private const string GAMEPAD_SCHEME_NAME = "Gamepad";
    
    public void OnMouseInput(InputAction.CallbackContext context)
    {
        if (_playerInput.currentControlScheme == MOUSE_AND_KEYBOARD_SCHEME_NAME)
        {
            Debug.Log("Using MnK");
        }
        else if (_playerInput.currentControlScheme == GAMEPAD_SCHEME_NAME)
        {
            Debug.Log("Using Gamepad");
        }
        else
            Debug.LogWarning("Warning: Unrecognised Scheme");
    }
}
