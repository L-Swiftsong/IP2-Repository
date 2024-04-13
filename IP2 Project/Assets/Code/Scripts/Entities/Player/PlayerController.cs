using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] public GameObject respawn;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.camera = GameManager.MainCamera;
    }


    public void Respawn() => GameManager.Instance.RespawnPlayer();
}
