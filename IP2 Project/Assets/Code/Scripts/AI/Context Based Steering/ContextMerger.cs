using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContextMerger : MonoBehaviour
{
    [SerializeField] private int _directionCount = 8;
    private Vector2[] _directions;

    private bool _hasCalculatedMaps;
    private float[] _interestMap;
    private float[] _dangerMap;


    [Space(10)]
    [SerializeField] private MergingMethod _mergingMethod;
    [System.Serializable]
    private enum MergingMethod
    {
        LowestValue,
        Subtraction
    }

    [Header("Debug")]
    [SerializeField] private bool _sendObstructionDebug;

    [Space(5)]
    [SerializeField] private bool _drawCombinedMaps;
    [SerializeField] private Color _interestColour = Color.green;
    [SerializeField] private Color _dangerColour = Color.red;


    private void Awake() => InitializeDirections();
    

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


    private void LateUpdate() => _hasCalculatedMaps = false;

    private void CalculateMaps(Vector2 position, Vector2 targetPos, BaseSteeringBehaviour[] behaviours)
    {
        // Initialise the arrays of all maps.
        List<float[]> interestMaps = new List<float[]>();
        List<float[]> dangerMaps = new List<float[]>();

        // Recieve the arrays of all maps.
        foreach(BaseSteeringBehaviour behaviour in behaviours)
        {
            interestMaps.Add(behaviour.GetInterestMap(position, targetPos, _directions));
            dangerMaps.Add(behaviour.GetDangerMap(position, targetPos, _directions));
        }


        // Collapse all interest maps into final interest map.
        _interestMap = CalculateFinalMap(interestMaps);

        // Collapse all danger maps into final danger map.
        _dangerMap = CalculateFinalMap(dangerMaps);


        _hasCalculatedMaps = true;
    }
    private float[] CalculateFinalMap(List<float[]> maps)
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


    public Vector2 CalculateBestDirection(Vector2 position, Vector2 targetPos, BaseSteeringBehaviour[] behaviours)
    {
        if (!_hasCalculatedMaps)
            CalculateMaps(position, targetPos, behaviours);


        float[] filteredDirections = CalculateFilteredDirections();


        // Find the direction with the greatest weight.
        float greatestDirection = Mathf.Max(filteredDirections);
        int targetIndex = System.Array.FindIndex(filteredDirections, t => t == greatestDirection);

        // Calculate the direction to move.
        Vector2 targetDirection = AverageDirections(filteredDirections, targetIndex);
        Debug.DrawRay(transform.position, targetDirection, Color.blue);

        return targetDirection;
    }
    private float[] CalculateFilteredDirections()
    {
        // Find the lowest danger that isn't 0.
        IEnumerable<float> nonZeroDangers = _dangerMap.Where(val => val > 0);
        float lowestDanger = nonZeroDangers.Count() > 0 ? nonZeroDangers.Min() : 0;
        Debug.Log(lowestDanger);


        // Filter out any directions where the value is greater than the lowest danger.
        float[] filteredDirections = new float[_directionCount];
        for (int i = 0; i < _directionCount; i++)
        {
            // Calculate the filtered direction.
            Debug.Log(i + ": " + _dangerMap[i]);
            if (_mergingMethod == MergingMethod.LowestValue)
            {
                if (_dangerMap[i] <= lowestDanger)
                    filteredDirections[i] = _interestMap[i];
            }
            else if (_mergingMethod == MergingMethod.Subtraction)
            {
                filteredDirections[i] = _interestMap[i] - _dangerMap[i];
            }

            if (_drawCombinedMaps)
                Debug.DrawRay(transform.position, Mathf.Abs(filteredDirections[i]) * _directions[i], filteredDirections[i] > 0 ? _interestColour : _dangerColour);
            else
            {
                Debug.DrawRay(transform.position, _dangerMap[i] * _directions[i], _dangerColour);
                Debug.DrawRay(transform.position, _interestMap[i] * _directions[i], _interestColour);
            }
        }


        return filteredDirections;
    }


    // Average directions in a filtered list, treating values below 0 as essentially blockades.
    Vector2 AverageDirections(float[] filteredDirections, int greatestDirectionIndex)
    {
        int minIndex = 0;
        int maxIndex = 0;
        int halfCount = Mathf.FloorToInt(_directionCount / 2);
        for (int i = -halfCount; i < halfCount; i++)
        {
            // Don't include the central value;
            if (i == 0)
                continue;

            // If the filtered value at this index is 0 (Has been excluded)
            int respectiveIndex = greatestDirectionIndex + (i < 0 ? i + _directionCount : i);
            if (respectiveIndex > _directionCount - 1)
                respectiveIndex -= _directionCount;
            else if (respectiveIndex < 0)
                respectiveIndex += _directionCount;


            if (filteredDirections[respectiveIndex] <= 0)
            {
                if (_sendObstructionDebug)
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
                if (_sendObstructionDebug)
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

        Vector2 averagedDirection = Vector2.zero;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            int respectiveIndex = greatestDirectionIndex + (i < 0 ? i + _directionCount : i);
            if (respectiveIndex > _directionCount - 1)
                respectiveIndex -= _directionCount;
            else if (respectiveIndex < 0)
                respectiveIndex += _directionCount;

            averagedDirection += filteredDirections[respectiveIndex] * _directions[respectiveIndex];
        }
        averagedDirection /= (maxIndex - minIndex) + 1;

        // If we have no direction and the largest direction is valid, select the largest direction as our target.
        if (averagedDirection == Vector2.zero && filteredDirections[greatestDirectionIndex] > 0)
            averagedDirection = filteredDirections[greatestDirectionIndex] * _directions[greatestDirectionIndex];

        return averagedDirection;
    }
}