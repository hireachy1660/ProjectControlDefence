using UnityEngine;
using System.Collections.Generic;
public class Grid : MonoBehaviour
{
    //public bool onlyDisplayPathGizmos;
    public bool displayGridGizmos; // 그리드를 그려주낟.
    public LayerMask UnwalkableMask; // Physics.Sphere에서 충돌을 감지할 Mask
    public LayerMask enemyUnit; 
    public LayerMask playerUnit;
    //public Transform player;
    public Vector2 gridWorldSize; // 그리드를 덮을 좌표 영역
    public float nodeRadius; // 노드 1개의 반지름
    private Node[,] grid;

    private float nodeDiameter; // 노드 1개의 지름
    private int gridSizeX, gridSizeY;
    

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

                bool inUnit = (Physics.CheckSphere(grid[x, y].worldPosition, nodeRadius, enemyUnit | playerUnit));
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
        float percentX = (_worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; // 그리드의 중심과 월드 포지션의 중심을 일치시키기 위해 비율로 계산
        float percentY = (_worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX); // 그리드 밖에 있는 경우 Clamp로 조정
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); // 배열은 [0]부터 시작하므로 -1을 계산
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

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) // 그리드 범위 내에 있는 이웃만 추가
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public bool CanUnitGrid(Node _node,float _unitSizeX,float _unitSizeZ) // 그리드를 중심으로 유닛 크기만큼의 주위 그리드들을 검사하여 사용여부를 판별
    {
        int sizeX = Mathf.RoundToInt(_unitSizeX / 2);
        int sizeZ = Mathf.RoundToInt(_unitSizeZ / 2);
        for (int x = -sizeX; x <= sizeX; ++x)
        {
            for (int y = -sizeZ; y <= sizeZ; ++y)
            {
                
                int checkX = _node.gridX + x;
                int checkY = _node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if(grid[checkX,checkY].inUnit || !grid[checkX,checkY].walkable)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                
            }
        }
        return true;
    }

    public bool TestUnitGrid(Vector3 checkPos, float radius)
    {
        LayerMask layerMask = enemyUnit | playerUnit;
        Collider[] colliders = Physics.OverlapSphere(checkPos, radius, layerMask);
        foreach (Collider col in colliders)
        {
            if (col.gameObject == this.gameObject)
            {
                continue;
            }
            return false;
        }
       
        return true;
    }



}
