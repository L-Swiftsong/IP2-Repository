using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    /// <summary>
    /// A* is a fairly flexible pathfinding solution.
    ///     This version of A* is setup solely for 2D, and aims to find the shortest unobstructed path from a Starting Position to a Target Position.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class Pathfinding : MonoBehaviour, IPathfinder
    {
        private Grid _grid;
        [SerializeField] private bool _orthogonalMovementOnly;

        private Heap<Node> _openSet;
        private HashSet<Node> _closedSet;

        private const int HORIZONTAL_MOVEMENT_COST = 10;
        private const int DIAGONAL_MOVEMENT_COST = 14;


        private void Awake()
        {
            // Get a reference to the grid.
            _grid = GetComponent<Grid>();
        }


        public void StartFindPath(Vector2 startPos, Vector2 targetPos, Action<Path> callback) => StartCoroutine(FindPath(startPos, targetPos, callback));

        private IEnumerator FindPath(Vector2 startPos, Vector2 targetPos, Action<Path> callback)
        {
            // Get the start and target nodes.
            Node startNode = _grid.NodeFromWorldPos(startPos);
            Node targetNode = _grid.NodeFromWorldPos(targetPos);


            // Create the Path.
            Path path = new Path();


            // If the target node is not walkable, then don't attempt pathfinding and immediately skip to returning the path.
            if (!targetNode.IsWalkable)
                goto ReturnPath;


            // If the open & closed sets haven't been initialised, initialise them.
            if (_openSet == null)
            {
                _openSet = new Heap<Node>(_grid.MaxSize);
                _closedSet = new HashSet<Node>();
            } else {
                // Otherwise, just clear the sets.
                _openSet.Clear();
                _closedSet.Clear();
            }


            // Add the starting node to the Open Set.
            _openSet.Add(startNode);


            // Loop through the nodes in the open set until there are none left.
            while (_openSet.Count > 0)
            {
                // Get the first node in the openSet Heap (It will have the lowest FCost in the Heap due to how it is structured).
                Node currentNode = _openSet.RemoveFirst();
                _closedSet.Add(currentNode);

                // If the current node is the target, we have found the target.
                if (currentNode == targetNode)
                {
                    path.IsValid = true;
                    break;
                }


                // Loop over each neighbour.
                foreach (Node neighbour in _orthogonalMovementOnly ? _grid.GetOrthogonalNeighbours(currentNode) : _grid.GetNodeNeighbours(currentNode))
                {
                    // Ignore non-walkable neighbours or neighbours that have already been evaluated.
                    if (!neighbour.IsWalkable || _closedSet.Contains(neighbour))
                        continue;

                    // Update the G & H costs of this neighbour.
                    UpdateNodeCost(currentNode, neighbour, targetNode);
                }
            }


        ReturnPath:
            yield return null;

            // If a path has been found, then set the path's waypoints.
            if (path.IsValid)
                path.Waypoints = RetracePath(startNode, targetNode);

            callback?.Invoke(path);
        }

        private void UpdateNodeCost(Node currentNode, Node neighbourNode, Node targetNode)
        {
            // Calculate the cost to move to the neighbour from this node.
            int newCostToNeightbour = currentNode.GCost + GetDistance(currentNode, neighbourNode);

            if (newCostToNeightbour < currentNode.GCost || !_openSet.Contains(neighbourNode))
            {
                // Update the G & H Costs of the neighbour.
                neighbourNode.GCost = newCostToNeightbour;
                neighbourNode.HCost = GetDistance(neighbourNode, targetNode);

                // Set the neighbour's parent to this node, as this is the shortest discovered distance to the node.
                neighbourNode.ParentNode = currentNode;
                if (!_openSet.Contains(neighbourNode))
                    _openSet.Add(neighbourNode);
            }
        }


        private Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Vector2> path = new List<Vector2>();

            // Work backwards from the endNode until we reach our startNode.
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                // Add the current node to the path.
                path.Add(currentNode.WorldPosition);

                // Set the current node to this node's parent.
                currentNode = currentNode.ParentNode;
            }

            // Simplify the path so that waypoints are only placed where the direction changes.
            path = SimplifyPath(path, startNode);

            // Our path is currently in reverse, so reverse it to make it the correct direction, then output it.
            path.Reverse();
            return path.ToArray();
        }

        // Generates only the waypoints from a list of nodes where the direction changes.
        private List<Vector2> SimplifyPath(List<Vector2> path, Node startNode)
        {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 oldDir = Vector2.zero;

            // Loop through each node in the path.
            for (int i = 1; i < path.Count; i++)
            {
                // Calculate the direction from the previous node to the current node.
                Vector2 newDir = path[i] - path[i - 1];

                // If the direction has changed, then add this node's position to the waypoint list.
                if (oldDir != newDir)
                {
                    waypoints.Add(path[i - 1]);
                    Debug.DrawLine(path[i - 1] + Vector2.up, path[i - 1] + Vector2.down, Color.red, 5f);
                }
                // Update the direction.
                oldDir = newDir;
            }

            if (oldDir != path[path.Count - 1] - startNode.WorldPosition)
                waypoints.Add(path[path.Count - 1]);

            // Return the waypoints list as an array.
            return waypoints;
        }


        private int GetDistance(Node a, Node b)
        {
            // Calculate the distance on each axis.
            int xDistance = Mathf.Abs(a.GridX - b.GridX);
            int yDistance = Mathf.Abs(a.GridY - b.GridY);
            
            // Calculate the number of steps required to reach the target node, subtracting steps for diagonals (The number of which are equal to the min between xDist and yDist).
            return HORIZONTAL_MOVEMENT_COST * (xDistance + yDistance) + ((DIAGONAL_MOVEMENT_COST - 2 * HORIZONTAL_MOVEMENT_COST) * Mathf.Min(xDistance, yDistance));
        }
    }
}