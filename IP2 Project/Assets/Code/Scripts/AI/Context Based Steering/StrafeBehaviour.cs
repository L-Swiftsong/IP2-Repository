using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Strafe", fileName = "New Strafe Behaviour")]
public class StrafeBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private bool _favourRight;


    public override float[] GetInterestMap(Vector2 position, Vector2 targetPos, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];
        
        // Cache Values.
        Vector2 targetDirection = (targetPos - position).normalized;
        

        // Loop through each direction.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the dot product.
            float dot = Vector2.Dot(targetDirection, directions[i]);
            bool toTheRight = Vector2.Dot(Vector2.Perpendicular(targetDirection), directions[i]) < 0;
            Debug.Log("Direction: " + directions[i] + " is " + (toTheRight ? "the Right" : "the Left"));

            // Weight the dot product to face the sides.
            float weightedDot = (1f - Mathf.Abs(dot)) * (toTheRight == _favourRight ? 1 : -1);

            // Assign our weighted dot as the interest.
            interestMap[i] = weightedDot;
        }

        return interestMap;
    }

    // Directions leading towards the target will be mapped to the danger map.
    public override float[] GetDangerMap(Vector2 position, Vector2 targetPos, Vector2[] directions) => new float[directions.Length];
}