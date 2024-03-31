using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private readonly List<ShakeRequest> _requests = new();
    // Note: ShakeRequest is a Class so that we can pass it by reference.
    private class ShakeRequest
    {
        public float ShakeIntensity;
        public float ShakeTime;
    }


    [SerializeField] private float _shakeDecreaseRate = 5f;

    private CinemachineBasicMultiChannelPerlin _cinemachineNoise;
    private void Awake() => _cinemachineNoise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    #region Event Subscription
    private void OnEnable()
    {
        PlayerCameraManager.OnRequestCameraShake += RequestShake;
        PlayerCameraManager.OnDisableCameraShake += DisableAllShakes;
    }
    private void OnDisable()
    {
        PlayerCameraManager.OnRequestCameraShake -= RequestShake;
        PlayerCameraManager.OnDisableCameraShake -= DisableAllShakes;
    }
    #endregion


    public void RequestShake(float intensity, float time = 0f)
    {
        if (intensity <= 0f)
            return;
        
        _requests.Add(
            new ShakeRequest
            {
                ShakeIntensity = intensity,
                ShakeTime = time
            });
    }
    private void DisableAllShakes()
    {
        // Set all active request times to 0, letting them decay naturally.
        for (int i = 0; i < _requests.Count; i++)
        {
            _requests[i].ShakeTime = 0f;
        }
    }


    private void Update()
    {
        // If we have no requests active, disable the noise and stop here.
        if (_requests.Count == 0)
        {
            _cinemachineNoise.m_AmplitudeGain = 0f;
            return;
        }


        // Find the strongest shake value.
        float largestIntensity = _requests.Max(t => t.ShakeIntensity);
        _cinemachineNoise.m_AmplitudeGain = largestIntensity;


        // Remove elapsed requests.
        for (int i = _requests.Count - 1; i >= 0; i--)
        {
            // Decrement shakeTime.
            ShakeRequest request = _requests[i];
            request.ShakeTime -= Time.deltaTime;

            // If the shake's time has elapsed, decrease its intensity towards 0.
            if (request.ShakeTime <= 0f)
            {
                request.ShakeIntensity -= _shakeDecreaseRate * Time.deltaTime;
                
                // If the request no longer has a positive intensity, remove it.
                if (request.ShakeIntensity <= 0f)
                    _requests.RemoveAt(i);
            }
        }
    }
}
