using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFaceDirection : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private Camera _playerCam;

    [Space(5)]
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private float _rotationSpeed;

    private Vector2? _mouseScreenPosition;
    private Vector2 _faceDirection;
    private Vector2 _movementDirection;

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
            if (context.canceled)
                _faceDirection = Vector2.zero;
            else
                _faceDirection = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            Debug.LogWarning("Warning: Invalid Scheme");
            _faceDirection = Vector2.zero;
        }
    }
    public void OnMovementInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
            _movementDirection = Vector2.zero;
        else
            _movementDirection = context.ReadValue<Vector2>();
    }


    private void Start() => _playerCam = GameManager.MainCamera;
    

    private void Update()
    {
        Vector2 targetDirection = Vector2.zero;

        // If we are using the mouse position, update the target direction every frame.
        //  This ensures that we keep facing the mouse even when the player moves but the mouse doesn't.
        if (_mouseScreenPosition.HasValue && _mouseScreenPosition.Value != Vector2.zero)
        {
            Vector3 mouseWorldPos = _playerCam.ScreenToWorldPoint(_mouseScreenPosition.Value);
            targetDirection = (mouseWorldPos - transform.position).normalized;
        }
        // If we aren't using the mousePos, try and use Gamepad input.
        else if (_faceDirection != Vector2.zero)
        {
            targetDirection = _faceDirection;
        }
        // Otherwise, try to use movement input.
        else
        {
            targetDirection = _movementDirection;
        }


        // If the targetDirection is still Vector2.zero, there was no usable input.
        if (targetDirection == Vector2.zero)
            return;


        // Rotate to face the target direction.
        _rotationPivot.rotation = Quaternion.RotateTowards(_rotationPivot.rotation, Quaternion.LookRotation(Vector3.forward, targetDirection), _rotationSpeed * Time.deltaTime);
    }
}
