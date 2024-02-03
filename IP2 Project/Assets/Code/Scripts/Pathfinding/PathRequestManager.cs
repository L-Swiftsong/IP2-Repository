using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        private IPathfinder _pathfindingMethod;
        private bool _isProcessingPath;
        
        
        private struct PathRequest
        {
            public Vector2 Start;
            public Vector2 End;
            public Action<Path> Callback;

            public PathRequest(Vector2 start, Vector2 end, Action<Path> callback)
            {
                this.Start = start;
                this.End = end;
                this.Callback = callback;
            }
        }
        private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;


        private static PathRequestManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _pathfindingMethod = GetComponent<IPathfinder>();
        }


        public static void RequestPath(Vector2 start, Vector2 end, Action<Path> callback)
        {
            PathRequest pathRequest = new PathRequest(start, end, callback);

            Instance._pathRequestQueue.Enqueue(pathRequest);
            Instance.TryProcessNext();
        }


        private void TryProcessNext()
        {
            // If we are currently processing a path or have no available paths, then return.
            if (_isProcessingPath || _pathRequestQueue.Count <= 0)
                return;

            // Update the current path request.
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;

            // Process the current path request.
            _pathfindingMethod.StartFindPath(_currentPathRequest.Start, _currentPathRequest.End, FinishedProcessingPath);
        }
        public void FinishedProcessingPath(Path path)
        {
            _currentPathRequest.Callback?.Invoke(path);
        
            // Process the next path.
            _isProcessingPath = false;
            TryProcessNext();
        }
    }
}