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
    public TMP_Text Kills;
    public TMP_Text Speed;

    int scoreTicker;
    int speedTicker;
    float timeElapsed;
    float AbilityTicker;
    float killTicker;
    float timeTickup;

    [SerializeField]private int scoreTimer;
    public float timer;


    // Start is called before the first frame update
    void Start()
    {

        scoreTicker = 0;
        speedTicker = 0;
        timeElapsed = 0;
        AbilityTicker = 0;
        killTicker = 0;
        timeTickup = 0;
        GameObject Player = GameObject.Find("Player Score");

        Speed.text = "0";
        Kills.text = "0";
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

        if (timeElapsed >= 0.4)
        {
            if (AbilityTicker < Player.GetComponent<PlayerScore>().finalNOU)
            {
                AbilityTicker += 1;
                Ability_Uses.text = AbilityTicker.ToString();
            }

            if (killTicker < Player.GetComponent<PlayerScore>().finalKills)
            {
                killTicker += 1;
                Kills.text = killTicker.ToString();
            }

            timeElapsed = 0;
        }

        if (AbilityTicker >= Player.GetComponent<PlayerScore>().finalNOU)
        {
            Ability_Uses.text = Player.GetComponent<PlayerScore>().finalNOU.ToString();
        }

        if (killTicker >= Player.GetComponent<PlayerScore>().finalKills)
        {
            Kills.text = Player.GetComponent<PlayerScore>().finalKills.ToString();
        }



        if (scoreTicker < Player.GetComponent<PlayerScore>().finalScore)
        {
            scoreTicker += 5;
            Score.text = scoreTicker.ToString();
        }

        if (speedTicker < Player.GetComponent<PlayerScore>().finalSpeedScore)
        {
            speedTicker += 3;
            Speed.text = speedTicker.ToString();
        }
        if (speedTicker >= Player.GetComponent<PlayerScore>().finalSpeedScore)
        {
            Speed.text = Player.GetComponent<PlayerScore>().finalSpeedScore.ToString();
        }


        if (scoreTicker >= Player.GetComponent<PlayerScore>().finalScore)
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
            timeTickup += Time.deltaTime * 30;
            Timer.text = timeTickup.ToString();
        }

        if (timeTickup >= Player.GetComponent<PlayerScore>().finalTime)
        {
            Timer.text = Player.GetComponent<PlayerScore>().finalTime.ToString();
        }

        
    }
}
