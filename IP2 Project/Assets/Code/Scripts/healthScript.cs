using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class healthScript : MonoBehaviour
{
    public int health = 0;
    public TMP_Text healthText;

    void Update()
    {
       healthText.text = "Health: " + health;
    }
}
