using System;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    /// <summary>
    /// https://youtu.be/mZfyt03LDH4?si=NedikF3tFgZEZVMh&t=1425
    /// </summary>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Node startNode = new Node(startPos, null, 0, 0);
        Node endNode = new Node(endPos, null, 0, 0);

        List<Node> openSet = new List<Node>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        openSet.Add(startNode);

        // THERE ARE NODES LEFT TO SEARCH.
        while (openSet.Count > 0)
        {
            // FIND CHEAPEST NODE IN OPEN.
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < node.FCost || openSet[i].FCost == node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node.position);
            Debug.Log($"1closing {node}");

            if (node == endNode)
                return RetracePath(startNode, endNode);

            foreach (Node neighbour in GetCardinalNeighbours(node, grid, endNode))
            {
                Debug.Log($"2neighbour{neighbour}");
                if (!IsNeighbourWalkable(node, neighbour, grid) || closedSet.Contains(neighbour.position))
                    continue;

                // this is wrong FIX !!!!
                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour >= neighbour.gCost && openSet.Contains(neighbour))
                    continue;

                neighbour.gCost = newCostToNeighbour;
                neighbour.hCost = GetDistance(neighbour, endNode);
                Debug.LogWarning(node != null);
                neighbour.parent = node;
                Debug.Log($"3set {neighbour} parent to {node}");
                Debug.Log($"4found waklable neighbour {neighbour}");
                if (openSet.Find(n => n.position == neighbour.position) == null)
                    openSet.Add(neighbour);
            }

            Debug.LogWarning("end of iteration");
        }

        Debug.LogWarning("NO PATH WAS FOUND !!");
        // THERE IS NO PATH.
        return null;
    }

    /// <summary>
    /// !OPTIMIZE 
    /// </summary>
    private List<Vector2Int> RetracePath(Node startNode, Node endNode) 
    {
        List<Node> nodePath = new List<Node>();
        Node currentNode = endNode;

        Debug.Log(startNode != null);
        Debug.Log(endNode != null);

        while (currentNode != startNode)
        {
            nodePath.Add(currentNode);
            Debug.LogWarning($"currentNode {currentNode}");
            Debug.Log($"currentNode.parent: {currentNode.parent}");
            currentNode = currentNode.parent;
        }

        nodePath.Reverse();

        List<Vector2Int> path = new List<Vector2Int>();
        nodePath.ForEach(x => path.Add(x.position));
        return path;
    }

    /// <summary>
    /// Here you can influence the heuristic.
    /// </summary>
    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.position.x - b.position.x);
        int distY = Mathf.Abs(a.position.y - b.position.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }

    private bool IsNeighbourWalkable(Node current, Node neighbour, Cell[,] grid)
    {
        return !grid[current.position.x, current.position.y].
            HasWall(ConvertDirToWall(neighbour.position - current.position));
    }

    private Wall ConvertDirToWall(Vector2Int dir)
    {
        if (dir == Vector2Int.right)
            return Wall.RIGHT;

        if (dir == Vector2Int.left)
            return Wall.LEFT;

        if (dir == Vector2Int.up)
            return Wall.UP;

        if (dir == Vector2Int.down)
            return Wall.DOWN;

        throw new Exception($"Direction invalid --> {dir}.");
    }

    /// <summary>
    /// Diagonals are excluded.
    /// </summary>
    private List<Node> GetCardinalNeighbours(Node node, Cell[,] grid, Node endNode)
    {
        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);
        
        List<Node> neightbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(z))
                    continue;

                int checkX = node.position.x + x;
                int checkZ = node.position.y + z;

                if (checkX < 0 || checkX >= gridWidth)
                    continue;

                if (checkZ < 0 || checkZ >= gridHeight)
                    continue;

                Vector2Int pos = new Vector2Int(checkX, checkZ);
                if (pos == endNode.position)
                {
                    neightbours.Add(endNode);
                }
                else
                {
                    neightbours.Add(new Node(new Vector2Int(checkX, checkZ), null, 0, 0));
                }
            }
        }

        return neightbours;
    }

    public class Node
    {
        /// <summary>
        /// Total distance of the path.
        /// </summary>
        public int FCost => gCost + hCost;

        public Vector2Int position;
        public Node parent; 

        /// <summary>
        /// Distance from starting node.
        /// </summary>
        public int gCost; 

        /// <summary>
        /// Guestimate to end node distance.
        /// </summary>
        public int hCost;

        public Node(Vector2Int position, Node parent, int gCost, int hCost)
        {
            this.position = position;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public override string ToString() => position.ToString();
    }
}
