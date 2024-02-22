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


        private EntityMovement _movementScript;
        [SerializeField] private List<Vector2> _patrolPoints;
        private int _currentPatrolPoint;

        private const float REACHED_POINT_THRESHOLD = 0.2f;


        [Space(5)]
        [SerializeField] private BaseSteeringBehaviour[] _movementBehaviours;


        [Tooltip("Determines whether this entity loops through patrol points sequentially (False) or randomly (True)")]
            [SerializeField] private bool _chooseRandomPoints;



        public void InitialiseValues(EntityMovement movementScript) => this._movementScript = movementScript;

        public override void Init()
        {
            base.Init();
            ResetPatrolPoint(randomValue: false);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            // Prevent errors from having no patrol points.
            if (_patrolPoints == null || _patrolPoints.Count == 0)
                return;


            // Detect if we've arrived at the patrol point.
            if (Vector2.Distance(_movementScript.transform.position, _patrolPoints[_currentPatrolPoint]) < REACHED_POINT_THRESHOLD)
            {
                // Select a new Patrol Point.
                SelectNextPatrolPoint(random: _chooseRandomPoints);
            }
            // Only move if we aren't at the patrol point.
            else
            {
                _movementScript.CalculateMovement(_patrolPoints[_currentPatrolPoint], _movementBehaviours, rotationType: RotationType.VelocityDirection);
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



        public void DrawGizmos(Transform gizmoOrigin, bool drawGizmos = true)
        {
            if (drawGizmos)
            {
                // Draw the patrol points.
                Gizmos.color = Color.white;
                foreach (Vector2 patrolPoint in _patrolPoints)
                {
                    Gizmos.DrawWireSphere(patrolPoint, 0.5f);
                }
            }

            foreach(BaseSteeringBehaviour behaviour in _movementBehaviours)
                behaviour.DrawGizmos(gizmoOrigin);
        }
    }
}