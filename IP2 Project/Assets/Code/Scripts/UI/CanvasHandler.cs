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

        gameObject.transform.GetChild(7).gameObject.SetActive(false);
    }
}
