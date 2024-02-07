using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.AStar
{
    public class Node : IHeapItem<Node>
    {
        public bool IsWalkable;
        public Vector2 WorldPosition;
        public int GridX;
        public int GridY;


        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;

        private int _heapIndex;


        public Node ParentNode;


        public Node(bool isWalkable, Vector2 worldPosition, int gridX, int gridY)
        {
            this.IsWalkable = isWalkable;
            this.WorldPosition = worldPosition;
            this.GridX = gridX;
            this.GridY = gridY;
        }



        public int HeapIndex { get => _heapIndex; set => _heapIndex = value; }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
                compare = HCost.CompareTo(nodeToCompare.HCost);

            return -compare;
        }
    }
}