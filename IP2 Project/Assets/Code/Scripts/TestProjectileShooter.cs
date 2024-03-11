using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectileShooter : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _ignoredTransform;

    [SerializeField] private float _initialDelay;
    [SerializeField] private float _delayBetweenShots;
    private float _nextShotTime;

   

    private void Start() =>_nextShotTime = Time.time + _initialDelay;
    private void Update()
    {
        if (Time.time > _nextShotTime)
            Shoot();
    }

    private void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.LookRotation(Vector3.forward, transform.up)).GetComponent<Projectile>();
        projectile.Init(_ignoredTransform, DebugHit);

        _nextShotTime = Time.time + _delayBetweenShots;
    }
    private void DebugHit(Transform hit) => Debug.Log(this.name + " hit: " + hit.name);
}
