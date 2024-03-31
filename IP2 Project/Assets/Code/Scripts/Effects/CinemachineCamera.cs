using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachineBasicMultiChannelPerlin _cinemachineBasicNoise;

    private Coroutine _cameraShakeCoroutine;

    private void Awake()
    {
        _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineBasicNoise = _cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    #region Event Subscription
    private void OnEnable()
    {
        PlayerCameraManager.OnRequestCameraShake += EnableCameraShake;
        PlayerCameraManager.OnDisableCameraShake += DisableCameraShake;
    }
    private void OnDisable()
    {
        PlayerCameraManager.OnRequestCameraShake -= EnableCameraShake;
        PlayerCameraManager.OnDisableCameraShake -= DisableCameraShake;
    }
    #endregion



    public void EnableCameraShake(float intensity, float time)
    {
        _cinemachineBasicNoise.m_AmplitudeGain = intensity;
        
        if (_cameraShakeCoroutine != null)
            StopCoroutine(_cameraShakeCoroutine);

        _cameraShakeCoroutine = StartCoroutine(DisableShakeAfterTime(time));
    }
    private IEnumerator DisableShakeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DisableCameraShake();
    }

    public void DisableCameraShake()
    {
        _cinemachineBasicNoise.m_AmplitudeGain = 0f;
    }
}
