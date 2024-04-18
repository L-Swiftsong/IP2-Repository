using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [Header("Explosion Variables")]
    [SerializeField] private float _explosiveDelay;
    private float _creationTime;

    [Tooltip("Should this projectile explode when it collides with another collider?")]
        [SerializeField] private bool _explodeOnCollision;
    // For destroying a projectile when it takes damage, add a HealthComponent to the projectile, tying the OnDeath() UnityEvent to Explode().

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;


    [Space(5)]
    [SerializeField] private float _explosionRadius;
    [SerializeField] private bool _earlyExplosionReducesSize;


    [Space(5)]
    [SerializeField] private GameObject _radiusViewerPrefab;
    private AoERadiusViewer _radiusViewerInstance;


    [Header("Explosion Effect")]
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _explosionEffectLifetime;


    [Header("Environmental Collisions")]
    [SerializeField] private LayerMask _environmentMask = 1 << 6;
    [SerializeField] private bool _reflectOnEnvironmentCollision;
    [SerializeField] private float _environmentReflectionMultiplier;

    // Explosion Collisions.
    private Transform _explosionIgnoredTransform;


    private Action<Transform[], Vector2> _explosionCallback;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ignoreTransform"></param>
    /// <param name="callback"></param>
    /// <param name="explosionDelay"></param>
    /// <param name="explodeOnCollision"></param>
    /// <param name="explosionRadius"></param>
    /// <param name="showRadius"></param>
    /// <param name="explosionCallback"></param>
    /// <param name="targetTransform"></param>
    /// <param name="targetPosition"></param>
    /// <param name="targetLayers"></param>
    /// <param name="ignoredFactions"></param>
    /// <param name="earlyExplosionReducesSize"></param>
    public void Init(Transform ignoreTransform, Action<Transform, Vector2> callback, float explosionDelay, bool explodeOnCollision, float explosionRadius, bool showRadius, Action<Transform[], Vector2> explosionCallback, Transform targetTransform = null, Vector2? targetPosition = null, LayerMask? targetLayers = null, Factions ignoredFactions = Factions.Unaligned, bool earlyExplosionReducesSize = false)
    {
        // Trigger the base Init().
        base.Init(
            ignoreTransform: ignoreTransform,
            callback: callback,
            targetTransform: targetTransform,
            targetPosition: targetPosition,
            targetLayers: targetLayers,
            ignoredFactions: ignoredFactions);


        // Set Values.
        this._explosiveDelay = explosionDelay;
        this._explodeOnCollision = explodeOnCollision;
        this._explosionRadius = explosionRadius;

        this._earlyExplosionReducesSize = earlyExplosionReducesSize;
        _creationTime = Time.time;

        this._explosionIgnoredTransform = ignoreTransform;


        // Set the explosion callback.
        this._explosionCallback = explosionCallback;


        // Create the radius indicator.
        if (showRadius && _radiusViewerPrefab != null)
        {
            _radiusViewerInstance = Instantiate<GameObject>(_radiusViewerPrefab, transform).GetComponent<AoERadiusViewer>();
            _radiusViewerInstance.Init(_explosionRadius, _explosiveDelay);
        }


        // Trigger the Automatic Explosion (Replace with a float that ticks down if we want to be able to reset the timer).
        Invoke(nameof(Explode), _explosiveDelay);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for whether the collision is a valid target.
        CheckForValidCollision(collision);

        // Check if the collision is environmental.
        CheckForEnvironmentalCollision(collision);
    }
    private void CheckForEnvironmentalCollision(Collider2D collision)
    {
        // Check for if the collider is an environmental one.
        if (_environmentMask != (_environmentMask | (1 << collision.gameObject.layer)))
            return;

        // If we are to reflect when we collide with the environment, do so.
        if (_reflectOnEnvironmentCollision)
        {
            Reflect(collision.transform, false);
            Speed *= _environmentReflectionMultiplier;
        }
        // Otherwise (Assuming that our delay is greater than 0 and we will eventually explode), stop movement.
        else if (_explosiveDelay > 0)
        {
            Speed = 0f;
            TargetTransform = null;
            TargetPosition = null;
        }
    }


    protected override void OnSuccessfulCollision(Transform hitTransform)
    {
        // Trigger the base collision functions (HitObject()).
        HitObject(hitTransform);

        // If we should explode on a successful collision, then do so.
        if (_explodeOnCollision)
            Explode();
    }

    public void Explode()
    {

        audioSource.PlayOneShot(audioClip);
        // Calculate the radius to be used for the explosion.
        float radiusLerp = _earlyExplosionReducesSize ? (Time.time - _creationTime) / _explosiveDelay : 1f;
        float radius = Mathf.Lerp(a: 0f, b: _explosionRadius, t: radiusLerp);
        
        // Get all valid (Unique) transforms within the explosion radius.
        HashSet<Transform> validTargets = new HashSet<Transform>();
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius, HitMask))
        {
            // Ignore the IgnoreTransform.
            if (collider.transform == _explosionIgnoredTransform)
                continue;
            
            // Ignore factions allied with one of the IgnoredFactions.
            if (collider.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
                if (entityFaction.IsAlly(IgnoredFactions))
                    continue;

            // Ensure that we don't hit entities that are obstructed.
            if (Physics2D.Linecast(transform.position, collider.transform.position, _environmentMask))
                continue;

            // Mark as valid for the _explosionCallback.
            validTargets.Add(collider.transform);
        }


        // Trigger the explosion callback.
        _explosionCallback?.Invoke(validTargets.ToArray(), transform.position);

        if (_explosionEffect != null)
        {
            // Create the explosion effect & destroy it after '_explosionEffectLifetime' seconds.
            Destroy(Instantiate<GameObject>(_explosionEffect, transform.position, Quaternion.identity), _explosionEffectLifetime);
        }

        // Destroy this projectile.
        DestroyProjectile();
    }


    protected override void DestroyProjectile()
    {
        // If the radius viewer exists, destroy it to ensure there are no errors (It should be a child of the projectile, but just in case).
        if (_radiusViewerInstance != null)
          Destroy(_radiusViewerInstance);


        // Destroy this gameobject.
        Destroy(this.gameObject);
    }
}
