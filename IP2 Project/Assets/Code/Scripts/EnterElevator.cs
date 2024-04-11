using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterElevator : MonoBehaviour
{
    public bool Entered;

    private void Start()
    {
        Entered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entered = false;
    }
}
