using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject vinyl;

    public string artist;

    public string songName;
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {

        vinyl.SetActive(true);

        vinyl.GetComponent<Record>().actualTime = vinyl.GetComponent<Record>().popUpTime;

        vinyl.GetComponent<Record>().text.GetComponent<TMP_Text>().text = artist + songName;
    }

   
}
