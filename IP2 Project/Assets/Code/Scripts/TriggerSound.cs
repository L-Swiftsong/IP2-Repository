using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    // Variables
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private LayerMask _playerLayer = 1 << 3;
    private bool playSound = true;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider that has entered this collision is not the player, return.
        if (_playerLayer != (_playerLayer | (1 << collision.gameObject.layer)))
            return;

        // Play audio source once
        if (playSound)
        {
            audioSource.Play();
            playSound = false;
        }
    }
}
