using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class healthScript : MonoBehaviour
{
    public int health = 0;
    public TMP_Text healthText;
    public GameObject Player;
    public GameObject Projectile;
    public GameObject RespawnPoint;

    void Update()
    {
        healthText.text = "Health: " + health;
        
        if (health <= 0)
        {
            Player.transform.position = RespawnPoint.transform.position;
            health = 5;
            Debug.Log("player died");
        }
    }
}
