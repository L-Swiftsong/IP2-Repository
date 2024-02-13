using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Patrol : State
    {
        public override string Name { get => "Patrolling"; }


        private Transform _transform;
        [SerializeField] private List<Vector2> _patrolPoints;
        private int _currentPatrolPoint;

        private const float REACHED_POINT_THRESHOLD = 0.2f;


        [Space(5)]
        [SerializeField] private float _patrolSpeed;
        [SerializeField] private float _rotateSpeed;


        [Tooltip("Determines whether this entity loops through patrol points sequentially (False) or randomly (True)")]
            [SerializeField] private bool _chooseRandomPoints;


        public void InitialiseValues(Transform transformToMove) => this._transform = transformToMove;

        public override void Init()
        {
            base.Init();
            ResetPatrolPoint(randomValue: false);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            // Detect if we've arrived at the patrol point.
            if (Vector2.Distance(_transform.position, _patrolPoints[_currentPatrolPoint]) < REACHED_POINT_THRESHOLD)
            {
                // Select a new Patrol Point.
                SelectNextPatrolPoint(random: _chooseRandomPoints);
            }
            // Only move if we aren't at the patrol point.
            else
            {
                // Calculate the direction to move.
                Vector2 directionToPoint = (_patrolPoints[_currentPatrolPoint] - (Vector2)_transform.position).normalized;
                
                // Move in the calculated direction.
                _transform.position += (Vector3)directionToPoint * _patrolSpeed * Time.deltaTime;

                // Rotate to face the direction of movement (Or in this case the target pos).
                Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, directionToPoint);
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
            }
        }


        private void SelectNextPatrolPoint(bool random = false)
        {
            if (random)
                _currentPatrolPoint = Random.Range(0, _patrolPoints.Count);
            else
            {
                if (_currentPatrolPoint >= _patrolPoints.Count - 1)
                    _currentPatrolPoint = 0;
                else
                    _currentPatrolPoint++;
            }
        }
        public void ResetPatrolPoint(bool randomValue = false)
        {
            if (randomValue)
                _currentPatrolPoint = Random.Range(0, _patrolPoints.Count);
            else
                _currentPatrolPoint = 0;
        }



        public void DrawGizmos()
        {
            Gizmos.color = Color.white;
            foreach (Vector2 patrolPoint in _patrolPoints)
            {
                Gizmos.DrawWireSphere(patrolPoint, 0.5f);
            }
        }
    }
}