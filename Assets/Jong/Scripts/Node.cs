using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public bool inUnit = false;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;

    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;

        gridX = _gridX;
        gridY = _gridY;
    }
   
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(Node _nodeToCompare)
    {
        int compare = fCost.CompareTo(_nodeToCompare.fCost); // fCost가 적을 수록 우선순위가 높다.
        if(compare == 0)
        {
            compare = hCost.CompareTo(_nodeToCompare.hCost);
        }
        return -compare;
    }
}
