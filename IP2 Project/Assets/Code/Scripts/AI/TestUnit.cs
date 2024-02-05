using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class TestUnit : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed = 5f;

    private Coroutine _followPathCoroutine;
    private Vector2[] _waypoints;
    private int _targetIndex;


    private void Start()
    {
        RequestPath();
    }


    [ContextMenu(itemName: "Request Path")]
    private void RequestPath() => PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);


    private void OnPathFound(Path path)
    {
        if (!path.IsValid)
            return;

        if (_followPathCoroutine != null)
            StopCoroutine(_followPathCoroutine);

        _waypoints = path.Waypoints;
        _followPathCoroutine = StartCoroutine(FollowPath());
    }
    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = new Vector3(_waypoints[0].x, _waypoints[0].y, transform.position.z);

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

                // Update the current waypoint.
                currentWaypoint = new Vector3(_waypoints[_targetIndex].x, _waypoints[_targetIndex].y, transform.position.z);
            }

            // Move towards the current waypoint.
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _moveSpeed * Time.deltaTime);

            yield return null;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (_waypoints != null && _waypoints.Length > 0)
        {
            for (int i = _targetIndex; i < _waypoints.Length; i++)
            {
                Gizmos.color = Color.yellow;
                if (i == _targetIndex)
                    Gizmos.DrawLine(transform.position, _waypoints[_targetIndex]);
                else
                    Gizmos.DrawLine(_waypoints[i - 1], _waypoints[i]);

                Gizmos.color = Color.black;
                Gizmos.DrawSphere(_waypoints[i], 0.2f);
            }
        }
    }
}
