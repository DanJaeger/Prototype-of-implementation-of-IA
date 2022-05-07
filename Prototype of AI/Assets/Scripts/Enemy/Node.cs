using UnityEngine;

public class Node : IHeapItem<Node>
{
    bool walkable;

    Vector3 worldPosition;
    int gridX;
    int gridY;

    int gCost;
    int hCost;
    int heapIndex;

    Node parent;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    public bool Walkable { get => walkable; }
    public Vector3 WorldPosition { get => worldPosition; }
    public int GCost { get => gCost; set => gCost = value; }
    public int HCost { get => hCost; set => hCost = value; }
    public int FCost { get => hCost + gCost; }
    public int GridX { get => gridX; set => gridX = value; }
    public int GridY { get => gridY; set => gridY = value; }
    public Node Parent { get => parent; set => parent = value; }
    public int HeapIndex { get => heapIndex; set => heapIndex = value; }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
