using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFaceDirection : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _playerCam;

    [SerializeField] private float _rotationSpeed;
    private Vector2 _targetDirection;

    private const string MOUSE_AND_KEYBOARD_SCHEME_NAME = "MnK";
    private const string GAMEPAD_SCHEME_NAME = "Gamepad";

    public void OnMouseInput(InputAction.CallbackContext context)
    {
        if (_playerInput.currentControlScheme == MOUSE_AND_KEYBOARD_SCHEME_NAME)
        {
            Vector3 worldPos = _playerCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            _targetDirection = (worldPos - transform.position).normalized;
        }
        else
        {
            Debug.LogWarning("Warning: Invalid Scheme");
            _targetDirection = Vector2.zero;
        }
    }
    public void OnFaceDirection(InputAction.CallbackContext context)
    {
        if (_playerInput.currentControlScheme == GAMEPAD_SCHEME_NAME)
        {
            Debug.Log("Using Gamepad");
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
        // Don't rotate if we have no input.
        if (_targetDirection == Vector2.zero)
            return;


        // Rotate to face the target direction.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, _targetDirection), _rotationSpeed * Time.deltaTime);

    }
}
