using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _hitMask;
    private ContactFilter2D _contactFilter;
    private List<Collider2D> _ignoredColliders;

    private Vector2 _previousPosition;
    private Action<Collider2D> _callback;

    [SerializeField] private bool _canReflect;


    public void Init(Collider2D ignoreCollider, Action<Collider2D> callback)
    {
        // Set the callback.
        this._callback = callback;

        // Initialize the contact filter.
        this._contactFilter = new ContactFilter2D();
        this._contactFilter.SetLayerMask(_hitMask);

        // Initialize the ignored colliders.
        this._ignoredColliders = new List<Collider2D>
        {
            ignoreCollider,
            this.GetComponent<Collider2D>()
        };

        // Initialize the previousPosition.
        _previousPosition = transform.position;
    }


    private void FixedUpdate()
    {
        transform.position += transform.up * _speed * Time.fixedDeltaTime;
        
        // Detect hit colliders.
        RaycastHit2D[] results = new RaycastHit2D[1];
        if (Physics2D.Linecast(_previousPosition, transform.position, _contactFilter, results) > 0)
            if (!_ignoredColliders.Contains(results[0].collider))
                HitObject(results[0].collider);
        

        // Update the previous position.
        _previousPosition = transform.position;
    }


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
    }


    private void HitObject(Collider2D hitCollider)
    {
        _callback?.Invoke(hitCollider);
        Destroy(this.gameObject);
    }
}
