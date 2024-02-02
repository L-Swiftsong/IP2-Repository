using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    public class Grid : MonoBehaviour
    {
        public List<Node> Path;


        [Header("Grid")]
        [SerializeField] private Vector2 _gridTopRight;
        [SerializeField] private Vector2 _gridBottomLeft;
        private Vector2 _gridCentre { get => (_gridTopRight + _gridBottomLeft) / 2f; }
        private Vector2 _gridExtents { get => _gridTopRight - _gridBottomLeft; }

        public int MaxSize => _gridSizeX * _gridSizeY;


        [Header("Nodes")]
        [Tooltip("The radius that each node will occupy")][SerializeField] private float _nodeRadius;
        private float _nodeDiameter;

        [SerializeField] private LayerMask _obstacleMask;


        private int _gridSizeX, _gridSizeY;
        private Node[,] _grid;


        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private float _nodeGizmoSize;

        [Space(5)]
        [SerializeField] private bool _drawOnlyPaths;



        private void Awake()
        {
            InitialiseGrid();
        }

        // Initialise the elements required to create the grid, and then create one.
        private void InitialiseGrid()
        {
            // Calculate the diameter of our nodes.
            _nodeDiameter = _nodeRadius * 2f;

            // Calculate how many nodes will be along the X axis and how many along the Y.
            Vector2 extents = _gridExtents;
            _gridSizeX = Mathf.RoundToInt(extents.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(extents.y / _nodeDiameter);

            CreateGrid();
        }
        // Create a new grid with the current grid parameters.
        private void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeY];

            // Loop over all the nodes.
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    // Calculate node values.
                    Vector2 worldPos = _gridBottomLeft + Vector2.right * (x * _nodeDiameter + _nodeRadius) + Vector2.up * (y * _nodeDiameter + _nodeRadius);
                    bool isWalkable = !(Physics2D.OverlapCircle(worldPos, _nodeRadius, _obstacleMask));

                    // Create the new node.
                    _grid[x, y] = new Node(isWalkable, worldPos, x, y);
                }
            }
        }


        public List<Node> GetNodeNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Don't add the centre node to the list of neighbours.
                    if (x == 0 && y == 0)
                        continue;

                    // Get the index of this node in the grid.
                    int nodeX = node.GridX + x;
                    int nodeY = node.GridY + y;

                    // Check if this node is within the grid's boundaries.
                    if ((nodeX >= 0 && nodeX < _gridSizeX) && (nodeY >= 0 && nodeY < _gridSizeX))
                    {
                        // If this is true, add this node to the list of neighbours.
                        neighbours.Add(_grid[nodeX, nodeY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPos(Vector2 worldPos)
        {
            Vector2 gridSize = _gridExtents;

            // Calculate the percentage of the X & Y coords where this node is within the grid.
            float xPercent = Mathf.Clamp01((worldPos.x / gridSize.x) + 0.5f);
            float yPercent = Mathf.Clamp01((worldPos.y / gridSize.y) + 0.5f);

            // Calculate the position of the corresponding node in the 2D array.
            int x = Mathf.RoundToInt((_gridSizeX - 1) * xPercent);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * yPercent);

            // Return the node.
            return _grid[x, y];
        }


        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
                return;
            
            // Draw the extents of the grid.
            Gizmos.DrawWireCube(_gridCentre, _gridExtents);

            if (_drawOnlyPaths)
            {
                if (Path != null)
                {
                    Gizmos.color = Color.yellow;
                    foreach (Node node in Path)
                    {
                        Gizmos.DrawCube(node.WorldPosition, Vector3.one * _nodeGizmoSize);
                    }
                }
            }
            else
            {
                // Draw the points on the grid.
                if (_grid != null)
                {
                    foreach (Node node in _grid)
                    {
                        Gizmos.color = node.IsWalkable ? Color.green : Color.red;
                        if (Path != null && Path.Contains(node))
                            Gizmos.color = Color.yellow;

                        Gizmos.DrawCube(node.WorldPosition, Vector3.one * _nodeGizmoSize);
                    }
                }
            }
        }
    }
}