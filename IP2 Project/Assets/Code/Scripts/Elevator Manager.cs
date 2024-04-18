using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ElevatorManager : MonoBehaviour
{
    [Header("Elevators")]
    public GameObject Elevator1;
    public GameObject Elevator2;
    public GameObject Elevator3;
    public GameObject Elevator4;

    public GameObject elevatorUI;
    public GameObject PlayerCanvas;
    public bool F;


    public GameObject Player;

    private void Start()
    {
        Player = GameObject.Find("Player");
        PlayerCanvas = GameObject.Find("Player Canvas");

        elevatorUI = PlayerCanvas.transform.GetChild(11).gameObject;

    }

    private void Update()
    {
        Floor1();
        Floor2();
        Floor3();
        Floor4();


        Player = GameObject.Find("Player");
        PlayerCanvas = GameObject.Find("Player Canvas");
        elevatorUI = PlayerCanvas.transform.GetChild(11).gameObject;
    }

    public void OnFPressed(InputAction.CallbackContext context)
    {
        if(Elevator1.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.gameObject.SetActive(true);
        }

        else if (Elevator2.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.gameObject.SetActive(true);
        }

        else if (Elevator3.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.gameObject.SetActive(true);
        }

        else if(Elevator4.GetComponent<EnterElevator>().Entered == true)
        {
            elevatorUI.gameObject.SetActive(true);
        }

        F = true;

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

            if(Player.GetComponent<PlayerCombo>().kills >= 7)
            {
                Player.transform.position = Elevator2.transform.position;
                
                elevatorUI.GetComponent<ElevatorButtons>().button2Pressed = false;
            }
            elevatorUI.SetActive(false);
        }
    }

    public void Floor3()
    {
        if (elevatorUI.GetComponent<ElevatorButtons>().button3Pressed == true)
        {
            if (Player.GetComponent<PlayerCombo>().kills >= 29)
            {
                Player.transform.position = Elevator3.transform.position;
                
                elevatorUI.GetComponent<ElevatorButtons>().button3Pressed = false;
            }
            elevatorUI.SetActive(false);


        }
    }

    public void Floor4()
    {
        if(elevatorUI.GetComponent<ElevatorButtons>().button4Pressed == true)
        {
            if (Player.GetComponent<PlayerCombo>().kills >= 35)
            {
                Player.transform.position = Elevator4.transform.position;
                
                elevatorUI.GetComponent<ElevatorButtons>().button4Pressed = false;
            }
                
        }
        elevatorUI.SetActive(false);
    }


}
