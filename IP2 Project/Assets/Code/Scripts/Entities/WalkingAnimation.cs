using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimation : MonoBehaviour
{
    [SerializeField] private GameObject rotationPivot;
    // Start is called before the first frame update
    void Start()
    {
        GameObject originalGameObject = GameObject.Find("Player");
        GameObject child = originalGameObject.transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
