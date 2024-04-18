using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButtons : MonoBehaviour
{
    public bool button1Pressed;

    public bool button2Pressed;

    public bool button3Pressed;

    public bool button4Pressed;

    private void Start()
    {
        button1Pressed = false;
        button2Pressed = false;
        button3Pressed = false;
        button4Pressed = false;
    }


    public void Button1()
    {
        button1Pressed = true;
    }

    public void Button2()
    {
        button2Pressed = true;
    }

    public void Button3()
    {
        button3Pressed = true;
    }

    public void Button4()
    {
        button4Pressed = true;
    }
}
