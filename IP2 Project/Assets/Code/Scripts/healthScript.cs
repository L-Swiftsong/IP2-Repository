using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthScript : MonoBehaviour
{
    public int health = 0;
    public Text healthText;

    void Update()
    {
       healthText.text = "health = " + health;
    }
}
