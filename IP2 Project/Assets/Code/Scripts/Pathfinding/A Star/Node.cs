using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    public class Node
    {
        public bool IsWalkable;
        public Vector2 WorldPosition;
        public int GridX;
        public int GridY;


        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;


        public Node ParentNode;


        public Node(bool isWalkable, Vector2 worldPosition, int gridX, int gridY)
        {
            this.IsWalkable = isWalkable;
            this.WorldPosition = worldPosition;
            this.GridX = gridX;
            this.GridY = gridY;
        }
    }
}