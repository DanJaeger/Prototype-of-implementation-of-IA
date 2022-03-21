using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] bool onlyDisplayPathGizmos;
    [SerializeField] Vector2 gridWorldSize = Vector2.zero; //Tamano global de la red = 50 x 18
    [SerializeField] float nodeRadius = 0.0f; // Tamano de los nodos = 0.5f
    [SerializeField] LayerMask unwalkableMask; 
    
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX; //Tamano global de la red en X = 50
    int gridSizeY; //Tamano global de la red en Y = 18

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
        float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y; 
        percentX = Mathf.Clamp01(percentX); 
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if(path != null)
            {
                foreach(Node node in path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
        else { 
        if(grid != null)
        {
            foreach(Node node in grid)
            {
                Gizmos.color = (node.Walkable) ? Color.white : Color.red;
                if(path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.green;
                    }
                }
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
        }
    }

}
