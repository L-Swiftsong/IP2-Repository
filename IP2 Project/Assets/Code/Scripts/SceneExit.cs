using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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


        GetCanvas.GetComponent<PlayerScore>().score += GetCanvas.GetComponent<PlayerScore>().speedScore;
        GetCanvas.GetComponent<PlayerScore>().finalScore = GetCanvas.GetComponent<PlayerScore>().score;
        GetCanvas.GetComponent<PlayerScore>().finalTime = GetCanvas.GetComponent<PlayerScore>().Timer;
        GetCanvas.GetComponent<PlayerScore>().finalGrade = GetCanvas.GetComponent<PlayerScore>().Grade;
        GetCanvas.GetComponent<PlayerScore>().finalSpeedScore = GetCanvas.GetComponent<PlayerScore>().speedScore;

        GetCanvas.GetComponent<PlayerScore>().speedScore = 0;
        GetCanvas.GetComponent<PlayerScore>().score = 0;
        GetCanvas.GetComponent<PlayerScore>().Timer = 0;

        Player = GameObject.Find("Player");

        GetCanvas.GetComponent<PlayerScore>().HighestScoreAchieved = Player.GetComponent<PlayerCombo>().HighestCombo;
        GetCanvas.GetComponent<PlayerScore>().finalKills = Player.GetComponent<PlayerCombo>().kills;
        GetCanvas.GetComponent<PlayerScore>().finalNOU = Player.GetComponent<AbilityHolder>().numberOfUses;
        Player.GetComponent<PlayerCombo>().score = 0;
        Player.GetComponent<AbilityHolder>().numberOfUses = 0;
        Player.GetComponent<PlayerCombo>()._currentComboProperty = 0;
        Player.GetComponent<PlayerCombo>().kills = 0;
        Player.GetComponent<PlayerCombo>().HighestCombo = 0;


    }
}
