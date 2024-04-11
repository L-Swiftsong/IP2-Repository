using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    
    public TMP_Text scoreText;
    public int score;
    public GameObject Player;
    [ReadOnly] public string Grade;
    [ReadOnly] public float Timer;

    [Header("Parameters For an S")]
    public float TimeForS;
    public int ScoreForS;
    public int AbilityUsesForS;

    [Header("Parameters For an A")]
    public float TimeForA;
    public int ScoreForA;
    public int AbilityUsesForA;

    [Header("Parameters For a B")]
    public float TimeForB;
    public int ScoreForB;
    public int AbilityUsesForB;

    [Header("Parameters For a C")]
    public float TimeForC;
    public int ScoreForC;
    public int AbilityUsesForC;

    [Header("Parameters For a D")]
    public float TimeForD;
    public int ScoreForD;
    public int AbilityUsesForD;

    private void Start()
    {
        score = 0;
        Player = GameObject.Find("Player");
        Player.GetComponent<PlayerCombo>().score = 0;
        Player.GetComponent<AbilityHolder>().numberOfUses = 0;
        Timer = 0;
    }
    void Update()
    {
        Timer += Time.deltaTime;
        int playerScore = Player.GetComponent<PlayerCombo>().score;
        int abilitieUses = Player.GetComponent<AbilityHolder>().numberOfUses;
        score = playerScore;

        scoreText.text = score.ToString() ;

        if (score == 0)
        {
            Grade = "F";
        }

        if(score >= ScoreForD && abilitieUses >= AbilityUsesForD && Timer <= TimeForD)
        {
            Grade = "D";
        }
        if (score>= ScoreForC && abilitieUses >= AbilityUsesForC && Timer <= TimeForC)
        {
            Grade = "C";
        }
        if(score >= ScoreForB && abilitieUses >= AbilityUsesForB && Timer <= TimeForB)
        {
            Grade = "B";
        }
        if (score >= ScoreForA && abilitieUses >= AbilityUsesForA && Timer <= TimeForA)
        {
            Grade = "A";
        }

        if (score >= ScoreForS && abilitieUses >= AbilityUsesForS && Timer <= TimeForS)
        {
            Grade = "S";
        }

        
    }
}