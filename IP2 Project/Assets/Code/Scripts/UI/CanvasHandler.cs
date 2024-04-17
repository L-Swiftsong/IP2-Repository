using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasHandler : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        _canvas.worldCamera = GameManager.MainCamera;
        
        
    }

    private void Start()
    {
        if (gameObject.name == "Player Canvas")
        {
            gameObject.transform.GetChild(11).gameObject.SetActive(false);
        }
    }
}
