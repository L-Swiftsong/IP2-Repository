using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("A temporary script used until we create a scene specifically for the player & their UI.")]
[RequireComponent(typeof(HealthComponent))]
public class PlayerHealthSubscriber : MonoBehaviour
{
    // Subscribe the PlayerHealthUI's UpdateHealth function to the player's HealthComponent's OnHealthChanged UnityEvent.
    private void Start()
    {
        // Find Components.
        HealthComponent playerHealth = GetComponent<HealthComponent>();
        PlayerHealthUI healthUI = FindObjectOfType<PlayerHealthUI>();

        // Subscribe to events.
        playerHealth.OnDamageTaken.AddListener(healthUI.UpdateHealth);
        playerHealth.OnHealingReceived.AddListener(healthUI.UpdateHealth);
    }
}
