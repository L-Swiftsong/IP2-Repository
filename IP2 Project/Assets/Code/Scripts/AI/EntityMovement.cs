using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour, IMoveable
{
    [SerializeField] private Rigidbody2D _rb2D;

    [Space(5)]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _rotationSpeed;

    [Space(10)]
    [SerializeField] private ContextMerger _ctxMerger;


    public void CalculateMovement(Vector2 targetPos, BaseSteeringBehaviour[] behaviours, RotationType rotationType = RotationType.None)
    {
        // Calculate the target direction.
        Vector2 targetDirection = _ctxMerger.CalculateBestDirection(_rb2D, targetPos, behaviours).normalized;

        // Move the rigidbody's velocity vector towards our desired velocity.
        _rb2D.velocity = Vector2.MoveTowards(_rb2D.velocity, targetDirection * _movementSpeed, _acceleration * Time.deltaTime);


        // Rotate using our desired method.
        Quaternion targetRot = transform.rotation;
        switch(rotationType)
        {
            case RotationType.VelocityDirection:
                targetRot = Quaternion.LookRotation(Vector3.forward, _rb2D.velocity);
                break;
            case RotationType.TargetDirection:
                Vector2 targetDir = (targetPos - (Vector2)transform.position).normalized;
                targetRot = Quaternion.LookRotation(Vector3.forward, targetDir);
                break;
        };
        
        // Commence rotation.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
    }
}

[System.Serializable]
public enum RotationType
{
    None = 0,
    VelocityDirection = 1,
    TargetDirection = 2
}