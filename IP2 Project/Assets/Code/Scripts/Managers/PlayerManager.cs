using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public static bool IsInitialised { get => Instance != null; }

    // Setup the Singleton Instance.
    private void Awake() => Instance = this;


    [SerializeField] private GameObject _player;
    public GameObject Player { get => _player; }

    public void SetPlayerPosition(Vector2 position) => _player.transform.position = new Vector3(position.x, position.y, _player.transform.position.z);
}
