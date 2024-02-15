using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistanceBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private Transform _target; // Our target

    [SerializeField] private float _targetDistance; // The distance that we wish to keep
    [SerializeField] private float _stopThreshold; // A threshold for when we should stop moving.
    [SerializeField] private float _maxValueThreshold; // A threshold representing the distance within which our interest decreases nearing the target. (Note: Only works without movement normalisation).


    [Header("Gizmos")]
    [SerializeField] private bool _drawGizmos;


    // Return a interest map based on the direction to the targets.
    public override float[] GetInterestMap(Vector2 position, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        // Cache values.
        Vector2 targetDirection = ((Vector2)_target.position - position).normalized;
        float distanceToTarget = Vector2.Distance(_target.position, position);


        // If we are within the stop threshold, then stop.
        if ((distanceToTarget > _targetDistance - _stopThreshold) && (distanceToTarget < _targetDistance + _stopThreshold))
            return interestMap;


        // Calculate the weight of this value.
        float targetWeight = Mathf.Lerp(
            a: -1,
            b: 1,
            t: (distanceToTarget - (_targetDistance - _maxValueThreshold)) / ((_targetDistance + _maxValueThreshold) - (_targetDistance - _maxValueThreshold)));

        // Loop through each direction we should consider.
        for (int i = 0; i < directions.Length; i++)
        {
            float interest = Vector2.Dot(targetDirection, directions[i]);

            // Scale interest based on distance.
            interest *= targetWeight;

            // If this interest is the largest in this direction slot, then assign it.
            if (interest > interestMap[i])
                interestMap[i] = interest;
        }
        

        // Return our calculated interest map.
        return interestMap;
    }

    // The ChaseBehaviour behaviour returns no danger map.
    public override float[] GetDangerMap(Vector2 position, Vector2[] directions) => new float[directions.Length];



    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _targetDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _targetDistance - _maxValueThreshold);
        Gizmos.DrawWireSphere(transform.position, _targetDistance + _maxValueThreshold);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _targetDistance - _stopThreshold);
        Gizmos.DrawWireSphere(transform.position, _targetDistance + _stopThreshold);
    }
}