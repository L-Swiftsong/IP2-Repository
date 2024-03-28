using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemToggle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    public void EnableEmitter() => _particleSystem.Play();
    public void DisableEmitter() => _particleSystem.Stop();
}
