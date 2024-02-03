using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    [RequireComponent(typeof(Grid), typeof(PathRequestManager))]
    public class Pathfinding : MonoBehaviour
    {
        private Grid _grid;
        private PathRequestManager _requestManager;

        private Heap<Node> _openSet;
        private HashSet<Node> _closedSet;

        private const int HORIZONTAL_MOVEMENT_COST = 10;
        private const int DIAGONAL_MOVEMENT_COST = 14;


        private void Awake()
        {
            // Get a reference to the grid & request manager.
            _grid = GetComponent<Grid>();
            _requestManager = GetComponent<PathRequestManager>();
        }


        public void StartFindPath(Vector2 startPos, Vector2 targetPos)
        {
            StartCoroutine(FindPath(startPos, targetPos));
        }


        private IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
        {
            // Get the start and target nodes.
            Node startNode = _grid.NodeFromWorldPos(startPos);
            Node targetNode = _grid.NodeFromWorldPos(targetPos);


            // Create the Path.
            Vector2[] wayPoints = new Vector2[0];
            bool wasPathSuccessful = false;


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
                    wasPathSuccessful = true;
                    break;
                }


                // Loop over each neighbour.
                foreach (Node neighbour in _grid.GetNodeNeighbours(currentNode))
                {
                    // Ignore non-walkable neighbours or neighbours that have already been evaluated.
                    if (!neighbour.IsWalkable || _closedSet.Contains(neighbour))
                        continue;

                    // Calculate the cost to move to the neighbour from this node.
                    int newCostToNeightbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                    if (newCostToNeightbour < currentNode.GCost || !_openSet.Contains(neighbour))
                    {
                        // Update the G & H Costs of the neighbour.
                        neighbour.GCost = newCostToNeightbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);

                        // Set the neighbour's parent to this node, as this is the shortest discovered distance to the node.
                        neighbour.ParentNode = currentNode;
                        if (!_openSet.Contains(neighbour))
                            _openSet.Add(neighbour);
                    }
                }
            }

            ReturnPath:
            yield return null;

            if (wasPathSuccessful)
                wayPoints = RetracePath(startNode, targetNode);

            _requestManager.FinishedProcessingPath(wayPoints, wasPathSuccessful);
        }
        private Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            
            // Work backwards from the endNode.
            Node currentNode = endNode;

            // Loop until we reach our start node.
            while (currentNode != startNode)
            {
                // Add the current node to the path.
                path.Add(currentNode);

                // Set the current node to this node's parent.
                currentNode = currentNode.ParentNode;
            }

            // Smooth the path into waypoints.
            Vector2[] waypoints = SmoothPath(path, startNode);

            // Our path is currently in reverse, so reverse it to make it the correct direction and then output it.
            Array.Reverse(waypoints);
            return waypoints;
        }

        // Generates only the waypoints from a list of nodes where the direction changes.
        private Vector2[] SmoothPath(List<Node> path, Node startNode)
        {
            List<Vector2> waypoints = new List<Vector2>();
            waypoints.Add(path[0].WorldPosition);

            Vector2 oldDir = Vector2.zero;

            // Loop through each node in the path.
            for (int i = 1; i < path.Count; i++)
            {
                // Calculate the direction from the previous node to the current node.
                Vector2 newDir = path[i].WorldPosition - path[i - 1].WorldPosition;

                // If the direction has changed, then add this node's position to the waypoint list.
                if (oldDir != newDir)
                    waypoints.Add(path[i].WorldPosition);
                
                // Update the direction.
                oldDir = newDir;
            }

            if (oldDir != path[path.Count - 1].WorldPosition - startNode.WorldPosition)
                waypoints.Add(path[path.Count - 1].WorldPosition);

            // Return the waypoints list as an array.
            return waypoints.ToArray();
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