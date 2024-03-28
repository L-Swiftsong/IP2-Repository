using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vinyl_Spin : MonoBehaviour
{
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.eulerAngles = new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.Rotate(0, 0, 10 * Time.deltaTime);
    }
}
