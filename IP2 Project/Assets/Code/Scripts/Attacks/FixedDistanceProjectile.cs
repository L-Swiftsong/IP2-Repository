using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedDistanceProjectile : MonoBehaviour
{
    [Header("Configurable Params")]
    [SerializeField] private Transform _currentRadiusIndicator;
    [SerializeField] private Transform _maxRadiusIndicator;
    
    [SerializeField] private AnimationCurve _distanceCurve;


    // Set by Init().
    private float _aoeRadius;
    private float _aoeDelay;
    private float _delayElapsed;
    private bool _hasDetonated;


    private Vector2 _startPos;
    private Vector2 _targetPos;
    private float _speed;


    private LayerMask _explosionHitMask;
    private Factions _ignoredFactions;
    private Collider2D _ignoredCollider;


    
    public void Init(Vector2 startPos, Vector2 targetPos, float speed, float radius, float delay, AnimationCurve throwCurve, LayerMask? damageableLayers = null, Collider2D ignoredCollider = null, Factions ignoredFactions = Factions.Unaligned)
    {
        this._startPos = startPos;
        this._targetPos = targetPos;
        this._speed = speed;
        transform.position = _startPos;

        
        this._aoeRadius = radius;
        this._aoeDelay = delay;
        _delayElapsed = 0f;
        _hasDetonated = false;
        
        float maxIndicatorScale = radius * 2f;
        _maxRadiusIndicator.localScale = new Vector3(maxIndicatorScale, maxIndicatorScale, maxIndicatorScale);


        if (throwCurve != null && throwCurve.keys[throwCurve.length - 1].value == 1)
            this._distanceCurve = throwCurve;


        if (damageableLayers.HasValue)
            this._explosionHitMask = damageableLayers.Value;
        this._ignoredFactions = ignoredFactions;
        this._ignoredCollider = ignoredCollider;
    }

    private void Start()
    {
        _delayElapsed = 0f;
        _hasDetonated = false;
    }


    private void Update()
    {
        // Movement.
        transform.position = Vector2.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);

        
        // Indicator.
        if (_delayElapsed < _aoeDelay)
        {
            if (_maxRadiusIndicator != null)
            {
                float maxScale = _aoeRadius * 2f;
                _maxRadiusIndicator.localScale = new Vector3(maxScale, maxScale, maxScale);
            }

            if (_currentRadiusIndicator != null)
            {
                float targetScale = Mathf.Lerp(0f, _aoeRadius, _delayElapsed / _aoeDelay) * 2f;

                if (targetScale == 0)
                    _currentRadiusIndicator.gameObject.SetActive(false);
                else
                {
                    _currentRadiusIndicator.gameObject.SetActive(true);
                    _currentRadiusIndicator.localScale = new Vector3(targetScale, targetScale, targetScale);
                }
            }

            _delayElapsed += Time.deltaTime;
        }
        else if (!_hasDetonated)
        {
            Explode();
            
            _hasDetonated = true;
        }
    }

    private void Explode()
    {
        // Loop through all colliders in range.
        foreach (Collider2D target in Physics2D.OverlapCircleAll(transform.position, _aoeRadius, _explosionHitMask))
        {
            // Ignore the ignored collider.
            if (target == _ignoredCollider)
                continue;
            
            // Ignore allied targets.
            if (target.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
                if (entityFaction.IsAlly(_ignoredFactions))
                    continue;


            // Deal damage.
            if (target.TryGetComponentThroughParents<HealthComponent>(out HealthComponent healthComponent))
                healthComponent.TakeDamage();
        }


        // Destroy this GO.
        Destroy(this.gameObject);
    }
}