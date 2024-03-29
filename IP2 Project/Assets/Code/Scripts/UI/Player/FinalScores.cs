using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class FinalScores : MonoBehaviour
{
    public TMP_Text Score;
    public TMP_Text Timer;
    public TMP_Text Grade;
    public TMP_Text Ability_Uses;

    int scoreTicker;
    float timeElapsed;
    float AbilityTicker;
    float timeTickup;
    [SerializeField]private int scoreTimer;
    public float timer;


    // Start is called before the first frame update
    void Start()
    {
        GameObject Player = GameObject.Find("Player Score");



        Score.text = "0";
        Grade.text = Player.GetComponent<PlayerScore>().finalGrade;
        Timer.text = "0";
        Ability_Uses.text = "0";


    }
    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        GameObject Player = GameObject.Find("Player Score");

        if (timeElapsed >= 0.4 && AbilityTicker < Player.GetComponent<PlayerScore>().finalNOU)
        {
            timeElapsed = 0;
            AbilityTicker += 1;
            Ability_Uses.text = AbilityTicker.ToString();
        }

        if (AbilityTicker >= Player.GetComponent<PlayerScore>().finalNOU)
        {
            Ability_Uses.text = Player.GetComponent<PlayerScore>().finalNOU.ToString();
        }

        if(scoreTicker < Player.GetComponent<PlayerScore>().finalScore)
        {
            scoreTicker += 3;
            Score.text = scoreTicker.ToString();
        }

        if(scoreTicker >= Player.GetComponent<PlayerScore>().finalScore)
        {
            Score.text = Player.GetComponent<PlayerScore>().finalScore.ToString();

            timer += Time.deltaTime;

            if (timer >= scoreTimer)
            {
                SceneManager.UnloadSceneAsync("Scoreboard");
                Player.GetComponent<PlayerScore>().Timer = 0;
            }
        }
        
        Grade.text = Player.GetComponent<PlayerScore>().finalGrade;

        if(timeTickup < Player.GetComponent<PlayerScore>().finalTime)
        {
            timeTickup += Time.deltaTime * 15;
            Timer.text = timeTickup.ToString();
        }

        if (timeTickup >= Player.GetComponent<PlayerScore>().finalTime)
        {
            Timer.text = Player.GetComponent<PlayerScore>().finalTime.ToString();
        }

        
    }
}
