using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private float ShakeIntensity = 10f;
    private float ShakeTime = 0.2f;

    private float timer;
    private CinemachineBasicMultiChannelPerlin cbmcp;
    // Start is called before the first frame update
    private void Start()
    {
        StopShake();
    }
    void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin cbmcp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmcp.m_AmplitudeGain = ShakeIntensity;

        timer = ShakeTime;
    }

    void StopShake()
    {
        CinemachineBasicMultiChannelPerlin cbmcp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmcp.m_AmplitudeGain = 0F;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            ShakeCamera();
        }
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                StopShake();
            }
        }
    }
}
