using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ICinemachineCamera))]
public class PlayerFollowCameraInitialiser : MonoBehaviour
{
    private ICinemachineCamera _camera;
    
    private void Awake()
    {
        _camera = GetComponent<ICinemachineCamera>();

        if (PlayerManager.IsInitialised)
            InitialiseCamera();
    }


    private void Update()
    {
        // If we haven't yet managed to set up the camera, try again.
        if (PlayerManager.IsInitialised)
            InitialiseCamera();
    }


    private void InitialiseCamera()
    {
        // Set the initial position of the camera.
        Vector3 startPos = PlayerManager.Instance.Player.transform.position;
        startPos.z = transform.position.z;
        transform.position = startPos;

        // Set the follow target of the camera.
        _camera.Follow = PlayerManager.Instance.Player.transform;

        // Destroy this script.
        Destroy(this);
    }
}
