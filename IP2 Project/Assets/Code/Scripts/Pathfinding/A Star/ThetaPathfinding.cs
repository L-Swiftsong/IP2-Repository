using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pathfinding.AStar.ThetaStar
{
    /// <summary>
    /// Theta* is a variation on A* that can efficiently compute paths with any angle,
    ///     as opposed to being constrained to the neighbours of the current node.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class ThetaPathfinding : MonoBehaviour, IPathfinder
    {
        private Grid _grid;

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
            Vector2[] waypoints = new Vector2[0];
            bool wasPathSuccessful = false;


            // If the target node is not walkable, then we shouldn't attempt pathfinding (We can later have this find the closest point on the grid)
            if (!targetNode.IsWalkable)
                goto ReturnPath;


            // If the open & closed sets haven't been initialised, initialise them.
            if (_openSet == null)
            {
                _openSet = new Heap<Node>(_grid.MaxSize);
                _closedSet = new HashSet<Node>();
            }
            else
            {
                // Otherwise, just clear the sets.
                _openSet.Clear();
                _closedSet.Clear();
            }


            // Add the starting node to the Open Set.
            _openSet.Add(startNode);
            startNode.ParentNode = startNode;


            // Loop through the nodes in the open set until there are none left.
            while(_openSet.Count > 0)
            {
                // Get the first node in the openSet Heap (Due to the structure of the heap, this will be the node with the lowest FCost).
                Node currentNode = _openSet.RemoveFirst();
                _closedSet.Add(currentNode);

                // If the current node is our target, we have found the target.
                if (currentNode == targetNode)
                {
                    Debug.Log("Path Found");
                    wasPathSuccessful = true;
                    break;
                }

                // Loop over each neighbour.
                foreach (Node neighbour in _grid.GetNodeNeighbours(currentNode))
                {
                    // Ignore non-walkable neighbours or neighbours that have already been evaluated.
                    if (!neighbour.IsWalkable || _closedSet.Contains(neighbour))
                        continue;

                    if(!_openSet.Contains(neighbour))
                    {
                        neighbour.GCost = int.MaxValue;
                        neighbour.ParentNode = null;
                    }

                    // Update the G & H costs of this neighbour.
                    UpdateNodeCost(currentNode, neighbour, targetNode);
                }
            }


        ReturnPath:
            yield return null;

            if (wasPathSuccessful)
                waypoints = RetracePath(startNode, targetNode);

            callback?.Invoke(new Path(waypoints, wasPathSuccessful));
        }

        private void UpdateNodeCost(Node currentNode, Node neighbourNode, Node targetNode)
        {
            // Cache the oldGCost of this node and compute the new cost.
            int oldGCost = neighbourNode.GCost;
            ComputeCost(currentNode, neighbourNode);

            // If the newCost is less than the old cost
            if (neighbourNode.GCost < oldGCost)
            {
                // Update the H Cost of the neighbour.
                neighbourNode.HCost = GetDistance(neighbourNode, targetNode);

                if (!_openSet.Contains(neighbourNode))
                    _openSet.Add(neighbourNode);
            }
        }
        private void ComputeCost(Node currentNode, Node neighbour)
        {
            // Path 2.
            if (currentNode.ParentNode != null && LineOfSight(currentNode.ParentNode, neighbour))
            {
                // Calculate the cost to move to the neighbour from the current node.
                int newCostFromParent = currentNode.ParentNode.GCost + GetDistance(currentNode.ParentNode, neighbour);

                // If the newCost is less than the current cost, then update the cost and parent node.
                if (newCostFromParent < neighbour.GCost)
                {
                    // Update the G Cost of the neighbour.
                    neighbour.ParentNode = currentNode.ParentNode;
                    neighbour.GCost = newCostFromParent;
                }
            }
            // Path 1.
            else
            {
                // Calculate the cost to move to the neighbour from the current node.
                int newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                // If the newCost is less than the current cost, then update the cost and parent node.
                if (newCostToNeighbour < neighbour.GCost)
                {
                    // Update the G Cost of the neighbour.
                    neighbour.ParentNode = currentNode;
                    neighbour.GCost = newCostToNeighbour;
                }
            }
        }


        private Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Vector2> waypoints = new List<Vector2>();

            // Work backwards from the endNode until we reach our startNode.
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                // Add the current node to the path.
                waypoints.Add(currentNode.WorldPosition);

                // Set the current node to this node's parent.
                currentNode = currentNode.ParentNode;
            }

            // Our path is currently in reverse, so reverse it to make it the correct direction and then output it.
            waypoints.Reverse();
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
        private bool LineOfSight(Node startNode, Node targetNode)
        {
            //return Physics2D.Linecast(a.WorldPosition, b.WorldPosition, _grid.ObstacleMask);

            // Cache values.
            int currentX = startNode.GridX; // Our current x position.
            int currentY = startNode.GridY; // Our current y position.
            int targetX = targetNode.GridX; // Our target x position.
            int targetY = targetNode.GridY; /// Our target y position.

            int xDiff = targetX - currentX; // Difference in X Positions between the current (Starting) & target X positions.
            int yDiff = targetY - currentY; // Difference in Y Positions between the current (Starting) & target Y positions.

            int f = 0; 

            int xMultiplier = 1; // A multiplier to control the sign of X (Pos vs Neg).
            int yMultiplier = 1; // A multiplier to control the sign of Y (Pos vs Neg).
            int xOffset = 0; // An offset applied to the current X when the xDifference is negative.
            int yOffset = 0; // An offset applied to the current Y when the yDifference is negative.


            // Set multipliers and offsets if the difference is negative.
            if (yDiff < 0)
            {
                yDiff *= -1;
                yMultiplier = -1;
                yOffset = -1;
            }
            if (xDiff < 0)
            {
                xDiff *= -1;
                xMultiplier = -1;
                xOffset = -1;
            }


            // If the xDifference is greater than the yDifference, then use yDifference as diagonal.
            if (xDiff >= yDiff)
            {
                // Loop until we reach our target.
                while (currentX != targetX)
                {
                    // Increment f.
                    f += yDiff;

                    // If f is greater than our xDifference, check diagonally.
                    if (f >= xDiff)
                    {
                        // Ensure node is walkable.
                        if (!_grid[currentX + xOffset, currentY + yOffset].IsWalkable)
                            return false;

                        // Increment currentY.
                        currentY += yMultiplier;
                        f -= xDiff;
                    }

                    // Ensure node is walkable.
                    if (f != 0 && !_grid[currentX + xOffset, currentY + yOffset].IsWalkable)
                        return false;

                    // Check sight overlaps two nodes. Ensure neither are unwalkable.
                    if (yDiff == 0 && !_grid[currentX + xOffset, currentY].IsWalkable && !_grid[currentX + xOffset, currentY - 1].IsWalkable)
                        return false;

                    // Increment currentX.
                    currentX += xMultiplier;
                }
            }
            // Otherwise, the yDifference is greater than the xDifference and we should use xDiff for the diagonal.
            else
            {
                // Loop until we reach our target.
                while (currentY != targetY)
                {
                    // Increment f.
                    f += xDiff;

                    // If f is greater than our yDifference, check diagonally.
                    if (f >= yDiff)
                    {
                        // Ensure node is walkable.
                        if (!_grid[currentX + xOffset, currentY + yOffset].IsWalkable)
                            return false;

                        // Increment currentX.
                        currentX += xMultiplier;
                        f -= yDiff;
                    }

                    // Ensure node is walkable.
                    if (f != 0 && !_grid[currentX + xOffset, currentY + yOffset].IsWalkable)
                        return false;

                    // Check sight overlaps two nodes. Ensure neither are unwalkable.
                    if (yDiff == 0 && !_grid[currentX, currentY + yOffset].IsWalkable && !_grid[currentX - 1, currentY + yOffset].IsWalkable)
                        return false;

                    // Increment currentY.
                    currentY += yMultiplier;
                }
            }

            // There were no obstructions, so the LoS is valid.
            return true;
        }
    }
}