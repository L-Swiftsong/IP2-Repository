using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    public class Grid : MonoBehaviour
    {
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
        [SerializeField] private LayerMask _walkableMask;

        [SerializeField, Range(1, 2)] private float _obstacleProximityPenalty;
        [SerializeField] private int _penaltyBlurSize = 3;


        private int _gridSizeX, _gridSizeY;
        private Node[,] _grid;
        public Node this[int x, int y]
        {
            get
            {
                if (_grid == null)
                    return null;

                return _grid[x, y];
            }
        }


        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private float _nodeGizmoSize;

        [SerializeField] private bool _drawPenalties;
        private int _minPenalty = int.MaxValue;
        private int _maxPenalty = int.MinValue;



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
                    bool isWalkable = !(Physics2D.OverlapCircle(worldPos, _nodeRadius * 0.9f, _obstacleMask));

                    // Calculate this node's Movement Penalty.
                    float movementPenalty = 0;
                    Collider2D[] col = Physics2D.OverlapCircleAll(worldPos, _nodeRadius, _walkableMask);
                    for (int i = 0; i < col.Length; i++)
                    {
                        if (col[i].TryGetComponent<WeightedCollider>(out WeightedCollider weightedCollider))
                            movementPenalty = Mathf.Max(movementPenalty, weightedCollider.Penalty);
                    }


                    // Increase the penalty for being near obstacles.
                    if (!isWalkable)
                        movementPenalty += _obstacleProximityPenalty;


                    // Create the new node.
                    _grid[x, y] = new Node(isWalkable, worldPos, x, y, movementPenalty);
                }
            }

            // Blur the penalties of the nodes.
            BluePenaltyMap(_penaltyBlurSize);
        }

        private void BluePenaltyMap(int blurSize)
        {
            if (blurSize == 0)
                return;
            
            // The size and bounds of our blending kernal.
            int kernelSize = (blurSize * 2) + 1; // The size of the kernal (Must be odd to have a central square).
            int kernelExtents = (kernelSize - 1) / 2; // The distance from the centre to the edge of the kernel.

            // 2D Arrays to store our various penalty passes.
            float[,] horizontalPenaltyPass = new float[_gridSizeX, _gridSizeY];
            float[,] verticalPenaltyPass = new float[_gridSizeX, _gridSizeY];
            

            // Horizontal Pass.
            for (int y = 0; y < _gridSizeY; y++)
            {
                // For the first node in each row we must loop through all nodes in the surrounding kernel.
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    horizontalPenaltyPass[0, y] += _grid[sampleX, y].MovementPenalty;
                }

                // For all other nodes, we can 
                for (int x = 1; x < _gridSizeX; x++)
                {
                    // Calculate the index of the node that is no longer in the kernel for this pass.
                    int removeIndex = Mathf.Clamp((x - kernelExtents) - 1, 0, _gridSizeX);
                    // Same as the removeIndex but for the nodes that have just entered the kernel
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);

                    // A.
                    horizontalPenaltyPass[x, y] = horizontalPenaltyPass[x - 1, y] - _grid[removeIndex, y].MovementPenalty + _grid[addIndex, y].MovementPenalty;
                }
            }


            // Vertical Pass (Note: As we have calculated the horizontal pass, we reference that rather than the grid to get the movement penalties).
            for (int x = 0; x < _gridSizeY; x++)
            {
                // For the first node in each row we must loop through all nodes in the surrounding kernel.
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    verticalPenaltyPass[x, 0] += verticalPenaltyPass[x, sampleY];
                }

                // Blur the bottom row.
                float blurredPenalty = (float)verticalPenaltyPass[x, 0] / (kernelSize * kernelSize);
                _grid[x, 0].MovementPenalty = blurredPenalty;

                // For all other nodes, we can 
                for (int y = 1; y < _gridSizeY; y++)
                {
                    // Calculate the index of the node that is no longer in the kernel for this pass.
                    int removeIndex = Mathf.Clamp((y - kernelExtents) - 1, 0, _gridSizeY);
                    // Same as the removeIndex but for the nodes that have just entered the kernel
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY - 1);

                    // A.
                    verticalPenaltyPass[x, y] = verticalPenaltyPass[x, y - 1] - horizontalPenaltyPass[x, removeIndex] + horizontalPenaltyPass[x, addIndex];

                    // Calculate the blurred penalty (The calculated penalty divided by the square of the kernel's size).
                    blurredPenalty = Mathf.RoundToInt((float)verticalPenaltyPass[x, y] / (kernelSize * kernelSize));

                    // (Gizmos).
                    //_maxPenalty = Mathf.Max(_maxPenalty, blurredPenalty);
                    //_minPenalty = Mathf.Min(_minPenalty, blurredPenalty);


                    // Set the corresponding node's
                    _grid[x, y].MovementPenalty = blurredPenalty;
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
                    if ((nodeX >= 0 && nodeX < _gridSizeX) && (nodeY >= 0 && nodeY < _gridSizeY))
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

            // Draw the points on the grid.
            if (_grid != null)
            {
                foreach (Node node in _grid)
                {
                    if (_drawPenalties)
                        Gizmos.color = node.IsWalkable ? Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(0, 1, node.MovementPenalty)) : Color.red;
                    else
                        Gizmos.color = node.IsWalkable ? Color.green : Color.red;


                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * _nodeGizmoSize);
                }
            }
        }
    }
}