using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    // 모든 오브젝트가 동시에 길 찾기를 수행하면 연산량이 많이 때문에 이를 관리해주기 위한 스크립트
    struct PathRequest
    {
        public Vector3 pathStart;  // 시작 위치
        public Vector3 pathEnd;  // 목적지
        public Action<Vector3[], bool> callback; // 경로를 찾고난 후 callback메소드(매개변수는 Vector3[] _path, 경로 탐색 성공여부 bool _success)

        public PathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[],bool> _callback)
        {
            pathStart = _pathStart;
            pathEnd = _pathEnd;
            callback = _callback;
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

    public static void CheckUnitGrid(Vector3 _unitPos, out bool _isUnitGrid)
    {
        _isUnitGrid = instance.pathFinding.isUnitGrid(_unitPos);
    }

    public static void CanUnitGrid(Vector3 _unitPos, float _unitSizeX, float _unitSizeZ, out bool _isUnitGrid)
    {
        _isUnitGrid = instance.pathFinding.CanUnitGrid(_unitPos, _unitSizeX, _unitSizeZ);
    }

    public static void TestUnitGrid(Vector3 _unitPos, float _radius, out bool _isUnitGrid)
    {
        _isUnitGrid = instance.pathFinding.TestUnitGrid(_unitPos, _radius);
    }

}
