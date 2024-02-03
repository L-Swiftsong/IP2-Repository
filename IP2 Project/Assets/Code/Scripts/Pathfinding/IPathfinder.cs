using System;
using UnityEngine;


namespace Pathfinding
{
    public interface IPathfinder
    {
        public void StartFindPath(Vector2 startPos, Vector2 endPos, Action<Path> callback);
    }
}