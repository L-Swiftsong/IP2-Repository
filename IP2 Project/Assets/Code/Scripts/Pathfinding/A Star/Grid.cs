using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Vector2 _gridTopRight;
    [SerializeField] private Vector2 _gridBottomLeft;
    private Vector2 _gridCentre { get => (_gridTopRight + _gridBottomLeft) / 2f; }
    private Vector2 _gridExtents { get => _gridTopRight - _gridBottomLeft; }


    [Header("Nodes")]
    [Tooltip("The radius that each node will occupy")] [SerializeField] private float _nodeRadius;
    private float _nodeDiameter;

    [SerializeField] private LayerMask _obstacleMask;


    private int _gridSizeX, _gridSizeY;
    private Node[,] _grid;


    private void Start()
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
                _grid[x, y] = new Node(isWalkable, worldPos);
            }
        }
    }


    public Node NodeFromWorldPos(Vector2 worldPos)
    {
        Vector2 gridSize = _gridExtents;

        // Calculate the percentage of the X & Y coords where this node is within the grid.
        float xPercent = Mathf.Clamp01((worldPos.x + (gridSize.x / 2f)) / gridSize.x);
        float yPercent = Mathf.Clamp01((worldPos.y + (gridSize.y / 2f)) / gridSize.y);

        // Calculate the position of the corresponding node in the 2D array.
        int x = Mathf.RoundToInt((_gridSizeX - 1) * xPercent);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * yPercent);

        // Return the node.
        return _grid[x, y];
    }


    private void OnDrawGizmos()
    {
        // Draw the extents of the grid.
        Gizmos.DrawWireCube(_gridCentre, _gridExtents);

        // Draw the points on the grid.
        if (_grid != null)
        {
            foreach (Node node in _grid)
            {
                Gizmos.color = node.IsWalkable ? Color.green : Color.red;
                Gizmos.DrawSphere(node.WorldPosition, _nodeRadius);
            }
        }
    }
}
