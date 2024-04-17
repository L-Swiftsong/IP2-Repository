using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    public GameObject Elevator1;
    public GameObject Elevator1Spawn;


    public GameObject Elevator2;
    public GameObject Elevator2Spawn;


    public GameObject Elevator3;
    public GameObject Elevator3Spawn;


    public GameObject Player;

    private void Start()
    {
        Player = GameObject.Find("Player");
    }


    private void Update()
    {
        if (Elevator1.GetComponent <EnterElevator>().Entered)
        {
            Player.transform.position = Elevator2Spawn.transform.position;
        }

        if (Elevator2.GetComponent<EnterElevator>().Entered)
        {
            Player.transform.position = Elevator1Spawn.transform.position;
        }

    }

}
