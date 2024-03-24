using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    
    public TMP_Text scoreText;
    private int score;
    public GameObject Player;
    [ReadOnly] public string Grade;
    
    [Header("Parameters For an A")]
    public int ScoreForA;
    public int AbilityUsesForA;

    [Header("Parameters For a B")]
    public int ScoreForB;
    public int AbilityUsesForB;

    [Header("Parameters For a C")]
    public int ScoreForC;
    public int AbilityUsesForC;

    [Header("Parameters For a D")]
    public int ScoreForD;
    public int AbilityUsesForD;

    private void Start()
    {
        score = 0;
        Player = GameObject.Find("Player");
        Player.GetComponent<PlayerCombo>().score = 0;
        Player.GetComponent<AbilityHolder>().numberOfUses = 0;
    }
    void Update()
    {
        int playerScore = Player.GetComponent<PlayerCombo>().score;
        int abilitieUses = Player.GetComponent<AbilityHolder>().numberOfUses;
        score = playerScore;

        scoreText.text = score.ToString() ;

        if (score == 0)
        {
            Grade = "F";
        }

        if(score >= ScoreForD && abilitieUses >= AbilityUsesForD)
        {
            Grade = "D";
        }
        if (score>= ScoreForC && abilitieUses >= AbilityUsesForC)
        {
            Grade = "C";
        }
        if(score >= ScoreForB && abilitieUses >= AbilityUsesForB)
        {
            Grade = "B";
        }
        if (score >= ScoreForA && abilitieUses >= AbilityUsesForA)
        {
            Grade = "A";
        }
    }
}
