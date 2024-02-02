using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool IsWalkable;
    public Vector2 WorldPosition;


    public Node(bool isWalkable, Vector2 worldPosition)
    {
        this.IsWalkable = isWalkable;
        this.WorldPosition = worldPosition;
    }
}
