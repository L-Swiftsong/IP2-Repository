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
            newUp = Vector2.Reflect(transform.up, reflectionTransform.up);

        // Set the new up.
        transform.up = newUp;


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
