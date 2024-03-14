using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private int _angleCount;


    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.camera = GameManager.MainCamera;
    }


    public Vector2 GetUp() => _rotationPivot.up;
    public Transform GetRotationPivot() => _rotationPivot;

    public float GetPlayerRotation()
    {
        float roundingValue = _angleCount > 0 ? 360f / _angleCount : 1;
        return Mathf.Round(_rotationPivot.eulerAngles.z / roundingValue) * roundingValue;
    }
}
