using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class TestUnit : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _updatePathDistance;

    [SerializeField] private float CHECK_TIME_DELAY;
    private float _nextCheckTime;


    [Space(5)]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 60f;

    private Coroutine _followPathCoroutine;
    private Vector2[] _waypoints;
    private int _targetIndex;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;



    //private void Start()
    //{
    //    RequestPath();
    //}

    private void Update()
    {
        if (/*Time.time >= _nextCheckTime ||*/ _waypoints == null || _waypoints.Length <= 0 || Vector2.Distance(_target.position, _waypoints[_waypoints.Length - 1]) >= _updatePathDistance)
        {
            RequestPath();
            _nextCheckTime = Time.time + CHECK_TIME_DELAY;
        }
    }


    [ContextMenu(itemName: "Request Path")]
    private void RequestPath() => PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);


    private void OnPathFound(Path path)
    {
        if (!path.IsValid)
            return;

        if (_followPathCoroutine != null)
            StopCoroutine(_followPathCoroutine);

        _targetIndex = 0;
        _waypoints = path.Waypoints;
        _followPathCoroutine = StartCoroutine(FollowPath());
    }
    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = new Vector3(_waypoints[0].x, _waypoints[0].y, transform.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, (_waypoints[0] - (Vector2)transform.position).normalized);

        while (true)
        {
            // If we are at the current waypoint, move onto the next.
            if (transform.position == currentWaypoint)
            {
                _targetIndex++;

                // If we have reached the end of our path, then stop looping.
                if (_targetIndex >= _waypoints.Length)
                {
                    _targetIndex = 0;
                    _waypoints = new Vector2[0];
                    yield break;
                }

                // Update the current waypoint & target direction.
                currentWaypoint = new Vector3(_waypoints[_targetIndex].x, _waypoints[_targetIndex].y, transform.position.z);
                targetRotation = Quaternion.LookRotation(Vector3.forward, (_waypoints[_targetIndex] - (Vector2)transform.position).normalized);
            }

            // Move towards the current waypoint.
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }


    private void OnDrawGizmos()
    {
        if (!_drawGizmos)
            return;
        
        if (_waypoints != null && _waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 1; i < _waypoints.Length; i++)
            {
                //if (i == _targetIndex)
                //    Gizmos.DrawLine(transform.position, _waypoints[_targetIndex]);
                //else
                    Gizmos.DrawLine(_waypoints[i - 1], _waypoints[i]);
            }
        }
    }
}
