using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public GameObject vinyl;

    // Start is called before the first frame update
    void Start()
    {
        vinyl = GameObject.Find("Player Canvas");

        vinyl = vinyl.transform.GetChild(0).gameObject;

        vinyl.SetActive(true);

        vinyl.GetComponent<Record>().actualTime = vinyl.GetComponent<Record>().popUpTime;
    }

   
}
