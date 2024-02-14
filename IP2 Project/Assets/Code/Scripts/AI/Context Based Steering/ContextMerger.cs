using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMerger : MonoBehaviour
{
    [SerializeField] private int _directionCount;
    private Vector2[] _directions;

    [SerializeField] private BaseSteeringBehaviour[] _behaviours;

    private float[] _interestMap;
    private float[] _dangerMap;


    [SerializeField] private float _speed;
    [SerializeField] private bool _move;


    [Header("Gizmos")]
    [SerializeField] private bool _drawMapTotals;
    [SerializeField] private Color _interestColour = Color.green;
    [SerializeField] private Color _dangerColour = Color.red;


    private void Awake()
    {
        InitializeDirections();
    }

    private void InitializeDirections()
    {
        // Recreate the directions array.
        _directions = new Vector2[_directionCount];

        // Calculate the direction interval.
        float directionInterval = (Mathf.PI * 2f) / _directionCount;

        // Calculate the directions
        for (int i = 0; i < _directionCount; i++)
        {
            // Calculate the angle of this direction (In Radians).
            float currentAngle = i * directionInterval;

            // Calculate this direction.
            _directions[i] = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle));
        }

        // Initialise the interest & danger maps.
        _interestMap = new float[_directionCount];
        _dangerMap = new float[_directionCount];
    }


    private void Update()
    {
        CalculateMaps();
        Vector2 movementDirection = CalculateBestDirection();

        if (_move)
            transform.position += (Vector3)movementDirection * _speed * Time.deltaTime;
    }


    private void CalculateMaps()
    {
        // Initialise the arrays of all maps.
        List<float[]> interestMaps = new List<float[]>();
        List<float[]> dangerMaps = new List<float[]>();

        // Recieve the arrays of all maps.
        foreach(BaseSteeringBehaviour behaviour in _behaviours)
        {
            interestMaps.Add(behaviour.GetInterestMap(transform.position, _directions));
            dangerMaps.Add(behaviour.GetDangerMap(transform.position, _directions));
        }


        // Collapse all interest maps into final interest map.
        _interestMap = CalculateFinalMap(interestMaps);

        // Collapse all danger maps into final danger map.
        _dangerMap = CalculateFinalMap(dangerMaps);
    }
    private float[] CalculateFinalMap(List<float[]> maps, bool combine = false)
    {
        float[] finalMap = new float[_directionCount];

        foreach (float[] map in maps)
        {
            // Loop through the directions of the current map.
            for (int i = 0; i < _directionCount; i++)
            {
                // Round the value so that small values (E.g. 0.1256E-8) don't throw off the result.
                float roundedValue = Mathf.RoundToInt(map[i] * 100) / 100f;

                // If the direction is larger than the maximum in this direction, then set that value of the final map.
                if (finalMap[i] < roundedValue)
                {
                    finalMap[i] = roundedValue;
                }
            }
        }
        

        return finalMap;
    }


    private Vector2 CalculateBestDirection()
    {
        // Find the lowest danger.
        float lowestDanger = _dangerMap[0];
        for (int i = 1; i < _directionCount; i++)
        {
            // If the danger is lower than the lowest, set the lowest.
            if (lowestDanger > _dangerMap[i])
                lowestDanger = _dangerMap[i];
        }
        
        // Filter out any directions where the value is greater than the lowest danger.
        float[] filteredDirections = new float[_directionCount];
        for (int i = 0; i < _directionCount; i++)
        {
            // Calculate the filtered direction.
            filteredDirections[i] = _interestMap[i] - _dangerMap[i];

            if (_drawMapTotals)
            {
                Debug.DrawRay(transform.position, _dangerMap[i] * _directions[i], _dangerColour);
                Debug.DrawRay(transform.position, _interestMap[i] * _directions[i], _interestColour);
            } else
                Debug.DrawRay(transform.position, Mathf.Abs(filteredDirections[i]) * _directions[i], filteredDirections[i] > 0 ? _interestColour : _dangerColour);
        }

        // Find the direction to move.
        float greatestDirection = Mathf.Max(filteredDirections);
        int targetIndex = System.Array.FindIndex(filteredDirections, t => t == greatestDirection);


        int minIndex = 0;
        int maxIndex = 0;
        int halfCount = Mathf.FloorToInt(_directionCount / 2);
        for (int i = -halfCount; i < halfCount; i++)
        {
            // Don't include the central value;
            if (i == 0)
                continue;

            // If the filtered value at this index is 0 (Has been excluded)
            int respectiveIndex = targetIndex + (i < 0 ? i + _directionCount : i);
            if (respectiveIndex > _directionCount - 1)
                respectiveIndex -= _directionCount;
            else if (respectiveIndex < 0)
                respectiveIndex += _directionCount;


            if (filteredDirections[respectiveIndex] <= 0)
            {
                Debug.Log(respectiveIndex + " is blocked (" + i + ")");

                // Reset the corresponding index.
                if (i < 0)
                    minIndex = 0;
                // If the corresponding index would be the max, then we can instead stop searching as we have reached the furthest extent in this direction.
                else
                    break;
            }
            // Otherwise, if it hasn't been excluded.
            else
            {
                Debug.Log(respectiveIndex + " is not blocked (" + i + ")");
                
                // Set the corresponding index.
                if (i < 0 && minIndex == 0)
                    minIndex = i;
                else
                    maxIndex = i;
            }
        }

        Debug.Log("Min: " + minIndex);
        Debug.Log("Max: " + maxIndex);

        Vector2 targetDirection = Vector2.zero;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            int respectiveIndex = targetIndex + (i < 0 ? i + _directionCount : i);
            if (respectiveIndex > _directionCount - 1)
                respectiveIndex -= _directionCount;
            else if (respectiveIndex < 0)
                respectiveIndex += _directionCount;

            targetDirection += filteredDirections[respectiveIndex] * _directions[respectiveIndex];
        }
        targetDirection /= (maxIndex - minIndex) + 1;

        // If we have no direction and the largest direction is valid, select the largest direction as our target.
        if (targetDirection == Vector2.zero && filteredDirections[targetIndex] > 0)
            targetDirection = filteredDirections[targetIndex] * _directions[targetIndex];

        Debug.DrawRay(transform.position, targetDirection, Color.blue);

        return targetDirection.normalized;
    }
}