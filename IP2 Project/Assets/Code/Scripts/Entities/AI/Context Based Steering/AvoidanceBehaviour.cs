using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Avoidance", fileName = "New Avoidance Behaviour")]
public class AvoidanceBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private float _detectionDistance = 2;
    [SerializeField] private LayerMask _obstacleLayers = 1 << 0 | 1 << 6 | 1 << 8; // Default Values: Default, Level, Entity.


    // The AvoidanceBehaviour behaviour does not return interest values.
    public override float[] GetInterestMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions) => new float[directions.Length];
    

    // Calculate danger values based on the distance to the obstacle.
    public override float[] GetDangerMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions)
    {
        float[] dangerMap = new float[directions.Length];

        // Loop through each potential target.
        foreach (Collider2D obstacle in Physics2D.OverlapCircleAll(movementBody.position, _detectionDistance, _obstacleLayers))
        {
            // Cache values.
            Vector2 closestPoint = obstacle.ClosestPoint(movementBody.position);
            Vector2 targetDirection = (closestPoint - movementBody.position).normalized;
            float targetDistance = Vector2.Distance(closestPoint, movementBody.position);

            // Calculate the weight applied to the weights, inversely proportional to the distance to the target.
            float targetWeight = 1f - (targetDistance / _detectionDistance);

            // Loop through each direction.
            for (int i = 0; i < directions.Length; i++)
            {
                // Calculate the danger using on the dot product, then weigh using the targetWeight.
                float dot = Vector2.Dot(targetDirection, directions[i]);
                float danger = Mathf.Clamp01(dot * targetWeight);

                if (danger > dangerMap[i])
                    dangerMap[i] = danger;
            }
        }

        return dangerMap; 
    }


    public override void DrawGizmos(Transform transform)
    {
        if (ShowGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionDistance);
    }
}