using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    [RequireComponent(typeof(Grid))]
    public class Pathfinding : MonoBehaviour
    {
        private Grid _grid;
        [SerializeField] private Transform _seeker, _target;

        private const int HORIZONTAL_MOVEMENT_COST = 10;
        private const int DIAGONAL_MOVEMENT_COST = 14;


        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }
        private void Update() => FindPath(_seeker.position, _target.position);


        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            // Get the start and target nodes.
            Node startNode = _grid.NodeFromWorldPos(startPos);
            Node targetNode = _grid.NodeFromWorldPos(targetPos);


            // Create the Open & Closed Sets.
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>(); // A Hashset is [Fill here].
            openSet.Add(startNode);

            // Loop through the nodes in the open set until there are none left.
            while(openSet.Count > 0)
            {
                // Loop through all nodes in the openSet and find the one with the lowest FCost.
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    // If this node has a lower FCost, then set it as the current node.
                    if (openSet[i].FCost < currentNode.FCost)
                        currentNode = openSet[i];
                    // If the FCosts are equal, choose the node with the lower HCost.
                    else if (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                        currentNode = openSet[i];
                }

                // Mark the current node as evaluated.
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // If the current node is the target, we have found the target.
                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }


                // Loop over each neighbour.
                foreach (Node neighbour in _grid.GetNodeNeighbours(currentNode))
                {
                    // Ignore non-walkable neighbours or neighbours that have already been evaluated.
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                        continue;

                    // Calculate the cost to move to the neighbour from this node.
                    int newCostToNeightbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                    if (newCostToNeightbour < currentNode.GCost || !openSet.Contains(neighbour))
                    {
                        // Update the G & H Costs of the neighbour.
                        neighbour.GCost = newCostToNeightbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);

                        // Set the neighbour's parent to this node, as this is the shortest discovered distance to the node.
                        neighbour.ParentNode = currentNode;
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        private void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            
            // Work backwards from the endNode.
            Node currentNode = endNode;

            // Loop until we reach our start node.
            while(currentNode != startNode)
            {
                // Add the current node to the path.
                path.Add(currentNode);

                // Set the current node to this node's parent.
                currentNode = currentNode.ParentNode;
            }

            // Our path is currently in reverse, so reverse it to make it the correct direction.
            path.Reverse();


            _grid.Path = path;
        }


        private int GetDistance(Node a, Node b)
        {
            // Calculate the distance on each axis.
            int xDistance = Mathf.Abs(a.GridX - b.GridX);
            int yDistance = Mathf.Abs(a.GridY - b.GridY);

            // If the distance on the X is greater than that on the Y, then the distance on the Y is the number of diagonal moves we must take.
            if (xDistance >= yDistance)
            {
                return DIAGONAL_MOVEMENT_COST * yDistance + HORIZONTAL_MOVEMENT_COST * (xDistance - yDistance);
            }
            // If the distance on the Y is greater than that on the X, then the distance on the X is the number of diagonal moves we must take.
            else
            {
                return DIAGONAL_MOVEMENT_COST * xDistance + HORIZONTAL_MOVEMENT_COST * (yDistance - xDistance);
            }
        }
    }
}