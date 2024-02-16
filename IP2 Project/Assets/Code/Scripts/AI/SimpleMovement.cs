using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;

    [Space(5)]
    [SerializeField] private ContextMerger _contextMerger;
    [SerializeField] private Transform _target;
    [SerializeField] private BaseSteeringBehaviour[] _behaviours;


    [Header("Further Options")]
    [SerializeField] private bool _shouldMove;
    [SerializeField] private bool _normalizeDirection;


    private void Update()
    {
        // Get the movement direction.
        Vector2 movementDirection = _contextMerger.CalculateBestDirection(transform.position, _target.position, _behaviours);
        if (_normalizeDirection)
            movementDirection.Normalize();


        // Move the velocity towards the targetVelocity.
        if (_shouldMove)
            _rb2D.velocity = Vector2.MoveTowards(_rb2D.velocity, movementDirection * _moveSpeed, _acceleration * Time.deltaTime);
    }
}
