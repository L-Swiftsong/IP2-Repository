using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFaceDirection : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _playerCam;

    [SerializeField] private float _rotationSpeed;
    private Vector2? _mouseScreenPosition;
    private Vector2 _targetDirection;

    private const string MOUSE_AND_KEYBOARD_SCHEME_NAME = "MnK";
    private const string GAMEPAD_SCHEME_NAME = "Gamepad";

    public void OnMouseInput(InputAction.CallbackContext context)
    {
        if (_playerInput.currentControlScheme == MOUSE_AND_KEYBOARD_SCHEME_NAME)
        {
            //Vector3 worldPos = _playerCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            _mouseScreenPosition = context.ReadValue<Vector2>();
        }
        else
        {
            Debug.LogWarning("Warning: Invalid Scheme");
            _mouseScreenPosition = null;
        }
    }
    public void OnFaceDirection(InputAction.CallbackContext context)
    {
        if (_playerInput.currentControlScheme == GAMEPAD_SCHEME_NAME)
        {
            _targetDirection = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            Debug.LogWarning("Warning: Invalid Scheme");
            _targetDirection = Vector2.zero;
        }
    }


    private void Start()
    {
        if (_playerCam == null)
            _playerCam = _playerInput.camera;
    }

    private void Update()
    {
        // If we are using the mouse position, update the target direction every frame.
        //  This ensures that we keep facing the mouse even when the player moves but the mouse doesn't.
        if (_mouseScreenPosition.HasValue)
        {
            Vector3 mouseWorldPos = _playerCam.ScreenToWorldPoint(_mouseScreenPosition.Value);
            _targetDirection = (mouseWorldPos - transform.position).normalized;
        }

        // Don't rotate if we have no input.
        if (_targetDirection == Vector2.zero)
            return;


        // Rotate to face the target direction.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, _targetDirection), _rotationSpeed * Time.deltaTime);
    }
}
