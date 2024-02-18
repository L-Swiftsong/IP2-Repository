using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class CBSViewer : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    [Space(5)]
    [SerializeField] private BaseSteeringBehaviour _behaviourToVisualise;
    [SerializeField] private int _directionCount;
    [SerializeField] private bool _combineMaps;
    
    private Vector2 _mousePos;

    private Vector2[] _directions;
    private float[] _interestMap;
    private float[] _dangerMap;


    public void OnMouseInput(InputAction.CallbackContext context)
    {
        Vector3 worldPos = _cam.ScreenToWorldPoint(context.ReadValue<Vector2>());
        _mousePos = worldPos;
    }


    private void Awake() => InitializeDirections();
    [ContextMenu("Initialize Directions")]
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



    void Update()
    {
        if (_behaviourToVisualise == null)
            return;
        
        // Get the interest and danger values.
        _interestMap = _behaviourToVisualise.GetInterestMap(transform.position, _mousePos, _directions);
        _dangerMap = _behaviourToVisualise.GetDangerMap(transform.position, _mousePos, _directions);

        // Visualise the directions.
        for (int i = 0; i < _directions.Length; i++)
        {
            if (_combineMaps)
            {
                float mapValue = _interestMap[i] - _dangerMap[i];
                Debug.DrawRay(transform.position, _directions[i] * mapValue, mapValue > 0 ? Color.green : Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, _directions[i] * _dangerMap[i], Color.red);
                Debug.DrawRay(transform.position, _directions[i] * _interestMap[i], Color.green);
            }
        }
    }
}
