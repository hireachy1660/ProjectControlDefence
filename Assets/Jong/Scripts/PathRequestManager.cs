using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    // 모든 오브젝트가 동시에 길 찾기를 수행하면 연산량이 많이 때문에 이를 관리해주기 위한 스크립트
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[],bool> _callback)
        {
            pathStart = _pathStart;
            pathEnd = _pathEnd;
            callback = _callback;
        }
    }

    struct GridResult
    {
        public Vector3 gridWorldPos;
        public bool isUnitGrid;

        public GridResult(Vector3 _gridWorldPos, bool _isUnitGrid)
        {
            gridWorldPos = _gridWorldPos;
            isUnitGrid = _isUnitGrid;
        }
    }

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    private static PathRequestManager instance; // 싱글턴 패턴

    PathFinding pathFinding;
    private bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }
    public static void RequestPath(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[],bool> _callback)
    {
        PathRequest newPathRequest = new PathRequest(_pathStart, _pathEnd, _callback);
        instance.pathRequestQueue.Enqueue(newPathRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }

    }

    public void FinishedProcessingPath(Vector3[] _path, bool _success)
    {
        currentPathRequest.callback(_path, _success);
        isProcessingPath = false;
        TryProcessNext();
    }

    public static void CheckUnitGrid(Vector3 _worldPosition, out bool _isUnitGrid)
    {
        _isUnitGrid = instance.pathFinding.isUnitGrid(_worldPosition);
    }

}
