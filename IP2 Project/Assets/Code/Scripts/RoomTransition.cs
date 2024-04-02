using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomTransition : MonoBehaviour
{
    [SerializeField] private Transform _roomTransitionPoint;
    [SerializeField] private LayerMask _playerLayer = 1 << 3;
    public GameObject Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider that has entered this collision is not the player, return.
        if (_playerLayer != (_playerLayer | (1 << collision.gameObject.layer)))
            return;

        // Set player variable
        Player = GameObject.Find("Player");

        // Teleport player to _roomTransitionPoint
        Player.transform.position = _roomTransitionPoint.transform.position;
    }
}
