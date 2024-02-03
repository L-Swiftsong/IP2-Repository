using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class TestUnit : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed = 5f;

    private Coroutine _followPathCoroutine;
    private Vector2[] _path;
    private int _targetIndex;


    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);
    }


    private void OnPathFound(Vector2[] path, bool wasSuccessful)
    {
        if (!wasSuccessful)
            return;

        if (_followPathCoroutine != null)
            StopCoroutine(_followPathCoroutine);

        _path = path;
        _followPathCoroutine = StartCoroutine(FollowPath());
    }
    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = new Vector3(_path[0].x, _path[0].y, transform.position.z);

        while (true)
        {
            // If we are at the current waypoint, move onto the next.
            if (transform.position == currentWaypoint)
            {
                _targetIndex++;

                // If we have reached the end of our path, then stop looping.
                if (_targetIndex >= _path.Length)
                {
                    _targetIndex = 0;
                    _path = new Vector2[0];
                    yield break;
                }

                // Update the current waypoint.
                currentWaypoint = new Vector3(_path[_targetIndex].x, _path[_targetIndex].y, transform.position.z);
            }

            // Move towards the current waypoint.
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _moveSpeed * Time.deltaTime);

            yield return null;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (_path != null && _path.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                if (i == _targetIndex)
                    Gizmos.DrawLine(transform.position, _path[_targetIndex]);
                else
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
            }
        }
    }
}
