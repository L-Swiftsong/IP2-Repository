using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public struct Path
    {
        public Vector2[] Waypoints { get; set; }
        public bool IsValid { get; }

        public Path(Vector2[] waypoints, bool isValid)
        {
            this.Waypoints = waypoints;
            this.IsValid = isValid;
        }
    }
}