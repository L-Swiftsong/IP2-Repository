using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterElevator : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayers;
    public bool Entered;

    private void Start()
    {
        Entered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerLayers != (_playerLayers | (1 << collision.gameObject.layer)))
            return;

        Entered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_playerLayers != (_playerLayers | (1 << collision.gameObject.layer)))
            return;

        Entered = false;
    }
}
