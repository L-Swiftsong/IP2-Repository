using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    public static Action<float, float> OnRequestCameraShake;
    public static Action OnDisableCameraShake;


    public void RequestShake(float intentity) => RequestShake(intentity, 0.1f);
    public void RequestShake(float intensity, float time) => OnRequestCameraShake?.Invoke(intensity, time);
    public void DisableActiveShake() => OnDisableCameraShake?.Invoke();
}
