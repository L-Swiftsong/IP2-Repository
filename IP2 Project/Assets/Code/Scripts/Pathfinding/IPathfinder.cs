using System;
using UnityEngine;


namespace Pathfinding
{
    /// <summary> An interface for pathfinding solutions. </summary>
    public interface IPathfinder
    {
        /// <summary> Request this pathfinding solution to start searching for a path.</summary>
        /// <param name="startPos"> The starting position of the path.</param>
        /// <param name="endPos"> The target position of the path.</param>
        /// <param name="callback"> The function called when a path has been found. (Note: Called whether the path is valid or not)</param>
        public void StartFindPath(Vector2 startPos, Vector2 endPos, Action<Path> callback);
    }
}