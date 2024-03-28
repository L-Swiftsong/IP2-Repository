using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTest : MonoBehaviour
{
    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.eulerAngles = new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        _rectTransform.Rotate(0, 0, 10 * Time.deltaTime);
        
    }
}
