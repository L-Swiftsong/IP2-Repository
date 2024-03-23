using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class Projectile : MonoBehaviour
{
    [Header("Projectile Movement")]
    [SerializeField] protected float Speed;
    public float ProjectileSpeed => Speed;

    [SerializeField] private float _rotationSpeed;
    protected Transform TargetTransform;
    private float _stopOrientationDelay = 0f;

    protected Vector2? TargetPosition;
    
    [Space(5)]
    [SerializeField] private float _maxLifetime = 7.5f;
    private float _destroyTime;


    [Header("Collision")]
    [SerializeField] private float _detectionOffset; 
    
    [SerializeField] protected LayerMask HitMask;
    [SerializeField] protected Factions IgnoredFactions;
    
    private List<Transform> _ignoredTransforms;

    private Action<Transform> _callback;


    [Header("Reflection")]
    [SerializeField] private bool _canReflect;
    private const float REFLECTION_AUTOTARGETING_PREVENTION_DURATION = 0.2f;


    /// <summary>
    /// Initialize the Projectile.
    /// </summary>
    /// <param name="ignoreTransform"></param>
    /// <param name="callback"></param>
    /// <param name="targetLayers"></param>
    /// <param name="ignoredFactions"></param>
    public void Init(Transform ignoreTransform, Action<Transform> callback, Transform targetTransform = null, Vector2? targetPosition = null, LayerMask? targetLayers = null, Factions ignoredFactions = Factions.Unaligned)
    {
        // Set the callback.
        this._callback = callback;

        // Set allied factions.
        this.IgnoredFactions = ignoredFactions;


        // Initialize the contact filter.
        if (targetLayers.HasValue)
            HitMask = targetLayers.Value;

        // Initialize the ignored colliders.
        this._ignoredTransforms = new List<Transform>
        {
            ignoreTransform
        };

        // Calculate the time when this projectile will be destroyed.
        _destroyTime = Time.time + _maxLifetime;
        
        
        // Setup specific targeting.
        this.TargetTransform = targetTransform;
        this.TargetPosition = targetPosition;
    }



    private void FixedUpdate()
    {
        // Decrement _stopOrientationDelay.
        if (_stopOrientationDelay > 0)
            _stopOrientationDelay -= Time.deltaTime;


        if (TargetTransform != null && _stopOrientationDelay <= 0f)
        {
            // Rotate towards the target transform.
            Vector2 targetDirection = (TargetTransform.position - transform.position).normalized;
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, _rotationSpeed * Time.deltaTime);
        }
        else if (TargetPosition.HasValue)
        {
            // Move towards the target position (Note: By using MoveTowards, we will never overshoot the target).
            transform.position = Vector2.MoveTowards(transform.position, TargetPosition.Value, Speed * Time.deltaTime);
            return;
        }
        
        
        // Move in our up direction.
        transform.position += transform.up * Speed * Time.fixedDeltaTime;
        
        // Destroy the projectile if the lifetime has elapsed.
        if (Time.time > _destroyTime)
            Destroy(this.gameObject);
    }

    public void Reflect(Transform reflectionTransform, bool reflectInFacingDirection = true)
    {
        // Don't reflect if we cannot be reflected.
        if (!_canReflect)
            return;


        // Clear the ignored colliders and assign the reflectionTransform.
        _ignoredTransforms.Clear();
        _ignoredTransforms.Add(reflectionTransform);

        // Set new ignored factions.
        if (reflectionTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            IgnoredFactions = entityFaction.Faction;
        else
            IgnoredFactions = Factions.Unaligned;


        // Calculate the new up vector
        Vector2 newUp;
        if (reflectInFacingDirection)
            newUp = reflectionTransform.up;
        else
        {
            // Set the reflection direction (In the case that we don't get the normal of the reflection transform.
            Vector2 reflectionDirection = reflectionTransform.up;


            // Calculate values for the linecast to find the normal of the reflectionTransform.
            Vector2 lineOrigin = transform.position - (transform.up * 2f * Time.deltaTime);
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = reflectionTransform.gameObject.layer;

            // Find the normal of the reflection transform.
            RaycastHit2D[] outHits = new RaycastHit2D[10];
            if (Physics2D.Linecast(lineOrigin, reflectionTransform.position, filter, outHits) > 0)
            {
                // If one of the hit objects is the reflectionTransform, use its normal as the reflection direction.
                Vector2 normal = outHits.First(t => t.transform == reflectionTransform).normal;
                reflectionDirection = normal != Vector2.zero ? normal : reflectionDirection;
            }

            // Reflect along the reflection direction.
            newUp = Vector2.Reflect(transform.up, reflectionDirection);
        }

        // Set the new up.
        transform.up = newUp;

        // Reset the destroy timer.
        _destroyTime = Time.time + _maxLifetime;


        // Improve reflection when we have a value for _targetTransform by stopping the autotargeting for a fixed delay.
        if (TargetTransform != null)
            _stopOrientationDelay = REFLECTION_AUTOTARGETING_PREVENTION_DURATION;
        
        // Allow reflection when we have a value for _targetPosition.
        if (TargetPosition.HasValue)
        {
            /*float remainingDistanceToTarget = Vector2.Distance(transform.position, _targetPosition.Value);
            Vector2 newTargetPosition = transform.position + transform.up * remainingDistanceToTarget;
            _targetPosition = newTargetPosition;*/            
            TargetPosition = null;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) => CheckForValidCollision(collision);
    

    protected void CheckForValidCollision(Collider2D collision)
    {
        // If the _hitMask does not contain the collided object's layer, then ignore it.
        if (HitMask != (HitMask | (1 << collision.gameObject.layer)))
            return;

        // Ignore collisions with already collided colliders.
        if (_ignoredTransforms.Contains(collision.transform))
            return;

        // Ignore factions allied with one of the _ignoredFactions.
        if (collision.transform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            if (entityFaction.IsAlly(IgnoredFactions))
                return;


        // The collision is a valid target.
        OnSuccessfulCollision(collision.transform);
    }
    protected virtual void OnSuccessfulCollision(Transform hitTransform)
    {
        HitObject(hitTransform);
        DestroyProjectile();
    }


    protected void HitObject(Transform hitTransform) => _callback?.Invoke(hitTransform);
    protected virtual void DestroyProjectile() => Destroy(this.gameObject);
}
