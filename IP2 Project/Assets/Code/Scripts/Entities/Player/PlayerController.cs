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


    public void Respawn()
    {

        gameObject.transform.position = respawn.transform.position;

        gameObject.GetComponent<HealthComponent>()._currentHealthProperty = gameObject.GetComponent<HealthComponent>()._maxHealthProperty;

        gameObject.GetComponent<HealthComponent>().Start();
    }


}
