using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] bool onlyDisplayPathGizmos;
    [SerializeField] Vector2 gridWorldSize = Vector2.zero; 
    [SerializeField] float nodeRadius = 0.5f;
    [SerializeField] LayerMask unwalkableMask; 
    
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY; 

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = 
            (transform.position - Vector3.right * gridWorldSize.x/2) - (Vector3.forward * gridWorldSize.y/2);

        #region Crear nodos en posicion correspondiente
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) +
                    Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y); 
            }
        }
        #endregion
    }
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neigbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neigbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neigbours;
    }
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x / gridWorldSize.x) + 0.5f;
        float percentY = (worldPosition.z / gridWorldSize.y) + 0.5f; 

        percentX = Mathf.Clamp01(percentX); 
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt(Mathf.Clamp(gridSizeX * percentX, 0, gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Clamp(gridSizeY * percentY, 0, gridSizeY - 1));

        return grid[x, y];
    }
    /*
    public Vector3[] path;
    public Node targetPosition;
    private void OnDrawGizmos()
    {
        List<Node> pathNodes = new List<Node>();

        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (path != null)
        {
            for (int i = 0; i < path.Length; i++)
            {
                pathNodes.Add(GetNodeFromWorldPoint(path[i]));
            }
        }

        if (onlyDisplayPathGizmos)
        {
            if(pathNodes != null)
            {
                foreach(Node node in pathNodes)
                {
                    Debug.Log(node);
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
        else {
            if (grid != null)
            {
                foreach (Node node in grid)
                {
                    Gizmos.color = (node.Walkable) ? Color.white : Color.red;
                    if (pathNodes.Contains(node))
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }
    */
}
