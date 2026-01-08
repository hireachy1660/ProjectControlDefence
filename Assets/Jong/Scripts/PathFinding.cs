using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System;

public class PathFinding : MonoBehaviour
{
    //public Transform seeker, target;
    private Grid grid;
    private PathRequestManager pathRequestManager;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    //private void Update()
    //{
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        FindPath(seeker.position, target.position);
    //    }
    //}
    private IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        Node startNode = grid.NodeFromWorldPoint(_startPos);
        Node targetNode = grid.NodeFromWorldPoint(_targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            //List<Node> openSet = new List<Node>();
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                //Node currentNode = openSet[0];
                Node currentNode = openSet.RemoveFirst();
                //for (int i = 1; i < openSet.Count; ++i)
                //{
                //    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                //    {
                //        currentNode = openSet[i];
                //    }
                //}
                //openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found : " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    
                    break;
                }
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closeSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.Update(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess);


    }
    private Vector3[] RetracePath(Node _startNode ,Node _endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = _endNode;

        while(currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

        //grid.path = path;
    }

    private Vector3[] SimplifyPath(List<Node> _path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i =1; i<_path.Count;++i)
        {
            Vector2 directionNew = new Vector2(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY);
            if(directionOld != directionNew)
            {
                waypoints.Add(_path[i - 1].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }
    private int GetDistance(Node _nodeA, Node _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int distY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);

    }

    public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        StartCoroutine(FindPath(_startPos, _targetPos));
    }
    public bool isUnitGrid(Vector3 _worldPosition)
    {
        Node currentNode = grid.NodeFromWorldPoint(_worldPosition);
        return currentNode.inUnit;
    }

}
