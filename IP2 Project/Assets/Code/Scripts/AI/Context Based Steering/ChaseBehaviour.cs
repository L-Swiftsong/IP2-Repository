using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private Transform[] _targets;
    [SerializeField] private float _maxChaseDistance;

    
    // Return a interest map based on the direction to the targets.
    public override float[] GetInterestMap(Vector2 position, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        // Loop through each potential target.
        foreach(Transform target in _targets)
        {
            // Cache values.
            Vector2 targetPos = target.position;
            Vector2 targetDirection = targetPos - position;
            float targetDistance = targetDirection.magnitude;
            targetDirection.Normalize();

            // Ignore targets that are too far.
            if (targetDistance > _maxChaseDistance)
                continue;


            // Loop through each direction we should consider.
            for(int i = 0; i < directions.Length; i++)
            {
                // Calculate the interest towards this point based on the dot product scaled to a range of 0-1.
                //float interest = (1f + Vector2.Dot(targetDirection, directions[i])) / 2f;
                float interest = Mathf.Clamp01(Vector2.Dot(targetDirection, directions[i]));

                // Scale interest based on distance.
                interest *= 1f - (targetDistance / _maxChaseDistance); 

                // If this interest is the largest in this direction slot, then assign it.
                if (interest > interestMap[i])
                    interestMap[i] = interest;
            }
        }

        // Return our calculated interest map.
        return interestMap;
    }

    // The ChaseBehaviour behaviour returns no danger map.
    public override float[] GetDangerMap(Vector2 position, Vector2[] directions) => new float[directions.Length];
    
}