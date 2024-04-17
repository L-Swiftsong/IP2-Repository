using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record : MonoBehaviour
{
    public float popUpTime;
    [ReadOnly]public float actualTime;

    public GameObject text;


    // Update is called once per frame
    void Update()
    {

        actualTime -= Time.deltaTime;

        if (actualTime <= 0)
        {
            gameObject.SetActive(false);
            text.SetActive(false);
        }

        if(actualTime > 0)
        {
            gameObject.SetActive(true);
            text.SetActive(true);
        }
    }
}
