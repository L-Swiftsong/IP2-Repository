using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChaseBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _maxChaseDistance;
    [SerializeField] private float _minChaseDistance;

    
    // Return a interest map based on the direction to the targets.
    public override float[] GetInterestMap(Vector2 position, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        // Cache values.
        Vector2 targetDirection = ((Vector2)_target.position - position).normalized;
        float distanceToTarget = Vector2.Distance(_target.position, position);

        // Ignore targets that are too far or too close.
        if (distanceToTarget > _maxChaseDistance || distanceToTarget < _minChaseDistance)
            return interestMap;

        //float targetWeight = (distanceToTarget / greatestDistance);

        // Loop through each direction we should consider.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the interest towards this point based on the dot product scaled to a range of 0-1.
            //float interest = (1f + Vector2.Dot(targetDirection, directions[i])) / 2f;
            float interest = Vector2.Dot(targetDirection, directions[i]);

            // Scale interest based on distance.
            //interest *= targetWeight;

            // If this interest is the largest in this direction slot, then assign it.
            if (interest > interestMap[i])
                interestMap[i] = interest;
        }
        

        // Return our calculated interest map.
        return interestMap;
    }

    // The ChaseBehaviour behaviour returns no danger map.
    public override float[] GetDangerMap(Vector2 position, Vector2[] directions) => new float[directions.Length];
    
}