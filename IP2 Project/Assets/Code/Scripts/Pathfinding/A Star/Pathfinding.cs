using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    [RequireComponent(typeof(Grid))]
    public class Pathfinding : MonoBehaviour
    {
        private Grid _grid;
        private Heap<Node> _openSet;
        private HashSet<Node> _closedSet;

        [SerializeField] private Transform _seeker, _target;

        private const int HORIZONTAL_MOVEMENT_COST = 10;
        private const int DIAGONAL_MOVEMENT_COST = 14;


        private void Awake()
        {
            // Get a reference to the grid.
            _grid = GetComponent<Grid>();
        }
        private void Start()
        {
            // Initialise the open & closed sets.
            _openSet = new Heap<Node>(_grid.MaxSize);
            _closedSet = new HashSet<Node>();
        }

        private void Update() => FindPath(_seeker.position, _target.position);


        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            // Get the start and target nodes.
            Node startNode = _grid.NodeFromWorldPos(startPos);
            Node targetNode = _grid.NodeFromWorldPos(targetPos);


            // Clear the Open & Closed Sets.
            _openSet.Clear();
            _closedSet.Clear();

            // Add the starting node to the Open Set.
            _openSet.Add(startNode);

            // Loop through the nodes in the open set until there are none left.
            while(_openSet.Count > 0)
            {
                // Get the first node in the openSet Heap (It will have the lowest FCost in the Heap due to how it is structured).
                Node currentNode = _openSet.RemoveFirst();
                _closedSet.Add(currentNode);

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
            
            // Calculate the number of steps required to reach the target node, subtracting steps for diagonals (The number of which are equal to the min between xDist and yDist).
            return HORIZONTAL_MOVEMENT_COST * (xDistance + yDistance) + ((DIAGONAL_MOVEMENT_COST - 2 * HORIZONTAL_MOVEMENT_COST) * Mathf.Min(xDistance, yDistance));
        }
    }
}