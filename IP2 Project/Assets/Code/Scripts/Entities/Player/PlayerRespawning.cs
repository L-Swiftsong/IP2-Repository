using System;
using System.Collections;
using UnityEngine;

public class PlayerRespawning : MonoBehaviour
{
    [SerializeField] private EntityAnimator _anim;
    public static Action OnRespawnReady;


    [Header("Objects to Destroy")]
    [SerializeField] private Collider2D _playerCollider;


    public void OnPlayerDied()
    {
        // Play the death animation.
        _anim.PlayDeadAnimation();
        GameManager.Instance.PauseLogic();

        // Notify that we are ready to respawn.
        OnRespawnReady?.Invoke();

        // Destroy the player's collider to prevent enemies from targeting them after death.
        StartCoroutine(DestroyComponents());
    }
    private IEnumerator DestroyComponents()
    {
        // Wait until logic reactivates.
        bool shouldContinue = false;
        GameManager.OnResumeLogic += () => shouldContinue = true;
        yield return new WaitUntil(() => shouldContinue);

        // Destroy the player once input has been reactivated.
        Destroy(_playerCollider);
    }
}
