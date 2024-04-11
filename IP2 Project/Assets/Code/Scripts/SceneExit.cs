using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SceneExit : MonoBehaviour
{
    [SerializeField] private SceneTransitionSO _transition;
    [SerializeField] private LayerMask _playerLayer = 1 << 3;
    public GameObject GetCanvas;
    public GameObject Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider that has entered this collision is not the player, return.
        if (_playerLayer != (_playerLayer | (1 << collision.gameObject.layer)))
            return;

        GameManager.Instance.CommenceTransition(_transition);

        GetCanvas = GameObject.Find("Player Score");
        GetCanvas.GetComponent<PlayerScore>().score = 0;
        GetCanvas.GetComponent<PlayerScore>().Timer = 0;

        Player = GameObject.Find("Player");
        Player.GetComponent<PlayerCombo>().score = 0;
        Player.GetComponent<AbilityHolder>().numberOfUses = 0;
        Player.GetComponent<PlayerCombo>()._currentComboProperty = 0;



    }
}
