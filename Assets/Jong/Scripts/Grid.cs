using UnityEngine;
using System.Collections.Generic;
public class Grid : MonoBehaviour
{
    //public bool onlyDisplayPathGizmos;
    public bool displayGridGizmos;
    public LayerMask UnwalkableMask; // Physics.Sphere에서 충돌을 감지할 Mask
    public LayerMask Unit;
    public LayerMask playerUnit;
    //public Transform player;
    public Vector2 gridWorldSize; // 그리드를 덮을 좌표 영역
    public float nodeRadius; // 노드 1개의 반지름
    private Node[,] grid;

    private float nodeDiameter; // 노드 1개의 지름
    int gridSizeX, gridSizeY;

    //public List<Node> path;
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); 
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

    }

    private void Update()
    {
        UpdateGrid();
    }
    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x<gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, UnwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }

    private void UpdateGrid()
    {
        if (grid == null) return;
        for (int x = 0; x < gridSizeX; ++x)
        {
            for(int y = 0; y < gridSizeY; ++y)
            {
                
                bool inUnit = (Physics.CheckSphere(grid[x,y].worldPosition, nodeRadius, Unit));
                inUnit = (Physics.CheckSphere(grid[x, y].worldPosition, nodeRadius, playerUnit));
                grid[x, y].inUnit = inUnit;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));

        //if(onlyDisplayPathGizmos)
        //{
        //    if(path != null)
        //    {
        //        foreach(Node n in path)
        //        {
        //            Gizmos.color = Color.black;
        //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
        //        }
        //    }
        //}
        //else
        //{
            if (grid != null && displayGridGizmos)
            {
                //Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    //if(playerNode == n)
                    //{
                    //    Gizmos.color = Color.cyan;
                    //}
                    //if (path != null)
                    //{
                    //    if (path.Contains(n))
                    //        Gizmos.color = Color.black;
                    //}
                    if(n.inUnit)
                {
                    Gizmos.color = Color.yellow;
                }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
       // }
      
    }

    public Node NodeFromWorldPoint(Vector3 _worldPosition)
    {
        float percentX = (_worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (_worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node _node)
    {
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x <= 1 ; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = _node.gridX + x;
                int checkY = _node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    
   
}
