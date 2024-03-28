using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        source.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
