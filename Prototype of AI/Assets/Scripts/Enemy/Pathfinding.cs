using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager pathRequestManager;
    Grid grid;

    [SerializeField] Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool pathSucces = false;

        Node startNode = grid.GetNodeFromWorldPoint(startPosition);
        Node targetNode = grid.GetNodeFromWorldPoint(endPosition);

        if (startNode.Walkable && targetNode.Walkable)
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSucces = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }

                }

            }
        }
        yield return null;
        if (pathSucces)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        pathRequestManager.FinishProcessingPath(wayPoints, pathSucces);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
            if (directionNew != directionOld)
            {
                wayPoints.Add(path[i].WorldPosition);
            }
            directionOld = directionNew;
        }
        return wayPoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
