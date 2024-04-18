using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [Header("Elevators")]
    public GameObject Elevator1;
    public GameObject Elevator2;
    public GameObject Elevator3;

    public GameObject elevatorUI;
    public GameObject PlayerCanvas;


    public GameObject Player;

    private void Start()
    {
        Player = GameObject.Find("Player");
        PlayerCanvas = GameObject.Find("Player Canvas");

        elevatorUI = PlayerCanvas.transform.GetChild(7).gameObject;

    }

    private void Update()
    {
        Floor1();
        Floor2();
        Floor3();
    }

    public void OnFPressed()
    {
        if(Elevator1.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.SetActive(true);
        }

        if (Elevator2.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.SetActive(true);
        }

        if (Elevator3.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.SetActive(true);
        }

        

    }

    public void Floor1()
    {
        if (elevatorUI.GetComponent<ElevatorButtons>().button1Pressed == true)
        {

            Player.transform.position = Elevator1.transform.position;
            elevatorUI.SetActive(false);
            elevatorUI.GetComponent<ElevatorButtons>().button1Pressed = false;

        }
    }


    public void Floor2()
    {
        if (elevatorUI.GetComponent<ElevatorButtons>().button2Pressed == true)
        {
            Player.transform.position = Elevator2.transform.position;
            elevatorUI.SetActive(false);
            elevatorUI.GetComponent<ElevatorButtons>().button2Pressed = false;
        }
    }

    public void Floor3()
    {
        if (elevatorUI.GetComponent<ElevatorButtons>().button3Pressed == true)
        {
            Player.transform.position = Elevator3.transform.position;
            elevatorUI.SetActive(false);
            elevatorUI.GetComponent<ElevatorButtons>().button3Pressed = false;
        }
    }


}
