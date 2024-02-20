using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Avoidance", fileName = "New Avoidance Behaviour")]
public class AvoidanceBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private float _detectionDistance;
    [SerializeField] private LayerMask _obstacleLayers;

    [Space(5)]
    [SerializeField] private float _interestDistance;


    // The AvoidanceBehaviour behaviour does not return interest values.
    public override float[] GetInterestMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        /*// Loop through each potential target.
        foreach (Collider2D obstacle in Physics2D.OverlapCircleAll(position, _interestDistance, _obstacleLayers))
        {
            // Cache values.
            Vector2 closestPoint = obstacle.ClosestPoint(position);
            Vector2 targetDirection = (closestPoint - position).normalized;
            float targetDistance = Vector2.Distance(closestPoint, position);

            // Calculate the weight applied to the weights, inversely proportional to the distance to the target.
            float targetWeight = 1f - (targetDistance / _interestDistance);

            // Loop through each direction.
            int applicationIndex = Mathf.CeilToInt(directions.Length / 2);
            int directionCount = directions.Length;
            for (int i = 0; i < directionCount; i++)
            {
                // Apply a shaping function to favour moving away from the target at a slight angle.
                float dot = Vector2.Dot(targetDirection, directions[i]);
                //float interest = -(1f - Mathf.Abs(dot + 0.65f));
                float interest = 1f - Mathf.Abs(dot + 0.75f);

                // Weigh the interest based on distance.
                interest *= targetWeight;

                // Apply the interest to the opposite direction.
                if (interest > interestMap[i])
                    interestMap[i] = interest;

                // Increase the application index, resetting to 0 if we exceed the number of values in the directions array.
                applicationIndex++;
                if (applicationIndex >= directionCount)
                    applicationIndex = 0;
            }
        }*/
        
        return interestMap;
    }

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
        Gizmos.DrawWireSphere(transform.position, _interestDistance);
        Gizmos.DrawWireSphere(transform.position, _detectionDistance);
    }
}