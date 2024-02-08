using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float _speed;


    [Header("Collision")]
    [SerializeField] private float _detectionOffset; 
    
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private Factions _ignoredFactions;
    private ContactFilter2D _contactFilter;
    
    private List<Collider2D> _ignoredColliders;

    private Action<Collider2D> _callback;


    [Header("Reflection")]
    [SerializeField] private bool _canReflect;


    public void Init(Collider2D ignoreCollider, Action<Collider2D> callback, Factions ignoredFactions = Factions.Unaligned)
    {
        // Set the callback.
        this._callback = callback;

        // Set allied factions.
        this._ignoredFactions = ignoredFactions;


        // Initialize the contact filter.
        this._contactFilter = new ContactFilter2D();
        this._contactFilter.SetLayerMask(_hitMask);

        // Initialize the ignored colliders.
        this._ignoredColliders = new List<Collider2D>
        {
            ignoreCollider,
        };
    }


    private void FixedUpdate() => transform.position += transform.up * _speed * Time.fixedDeltaTime;
    

    public void Reflect(Transform reflectionTransform, bool reflectInFacingDirection = true)
    {
        // Don't reflect if we cannot be reflected.
        if (!_canReflect)
            return;


        // Calculate the new up vector
        Vector2 newUp;
        if (reflectInFacingDirection)
            newUp = reflectionTransform.up;
        else
            newUp = Vector2.Reflect(transform.up, reflectionTransform.up);

        // Set the new up.
        transform.up = newUp;


        // Clear the ignored colliders and assign the reflectionTransform.
        _ignoredColliders.Clear();
        if (reflectionTransform.TryGetComponent<Collider2D>(out Collider2D reflectionCollider))
            _ignoredColliders.Add(reflectionCollider);

        // Set new ignored factions.
        if (reflectionTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            _ignoredFactions = entityFaction.Faction;
        else
            _ignoredFactions = Factions.Unaligned;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collisions with already collided colliders.
        if (_ignoredColliders.Contains(collision))
            return;

        // Ignore factions allied with one of the _ignoredFactions.
        if (collision.transform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            if (entityFaction.IsAlly(_ignoredFactions))
                return;
        

        HitObject(collision);
    }


    private void HitObject(Collider2D hitCollider)
    {
        _callback?.Invoke(hitCollider);
        Destroy(this.gameObject);
    }
}
