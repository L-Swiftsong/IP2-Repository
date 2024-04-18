using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public static bool IsInitialised { get => Instance != null; }

    private void Awake()
    {
        // Setup the Singleton Instance.
        Instance = this;


        if (_playerInput == null)
            _playerInput = _player.GetComponent<PlayerInput>();
    }

    [SerializeField] private GameObject _player;
    public GameObject Player { get => _player; }

    public void SetPlayerPosition(Vector2 position) => _player.transform.position = new Vector3(position.x, position.y, _player.transform.position.z);



    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

    public void RevokePlayerControl() => _playerInput.DeactivateInput();
    public void RegainPlayerControl() => _playerInput.ActivateInput();



    [SerializeField] private GameObject _playerCanvas;
    public GameObject PlayerCanvas => _playerCanvas;
}
