using System;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    /// <summary>
    /// https://youtu.be/mZfyt03LDH4?si=NedikF3tFgZEZVMh&t=1425
    /// Current bug: there is never a path. Working on it.
    /// </summary>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        // !FIX ??
        Node start = new Node(startPos, null, 0, 0);
        Node end = new Node(endPos, null, 0, 0);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(start);

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
            closedSet.Add(node);

            if (node == end)
                return RetracePath(start, end);

            foreach (Node neighbour in GetCardinalNeighbours(node, grid))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                if (!IsNeighbourWalkable(node, neighbour, grid))
                    continue;

                int newGCostNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newGCostNeighbour >= neighbour.gCost && openSet.Contains(neighbour))
                    continue;

                neighbour.gCost = newGCostNeighbour;
                neighbour.hCost = GetDistance(neighbour, end);
                neighbour.parent = node;
                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
            }
        }

        // THERE IS NO PATH.
        return null;
    }

    private List<Vector2Int> RetracePath(Node start, Node end) 
    {
        List<Node> nodePath = new List<Node>();
        Node node = end;

        while (node != start)
        {
            nodePath.Add(node);
            node = node.parent;
        }

        nodePath.Reverse();

        List<Vector2Int> path = new List<Vector2Int>();
        nodePath.ForEach(x => path.Add(x.position));
        return path;
    }

    /// <summary>
    /// Note to self:
    /// here you can add the heuretic bias.
    /// </summary>
    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.position.x - b.position.x);
        int distZ = Mathf.Abs(a.position.y - b.position.y);
        if (distX > distZ)
            return 14 * distZ + 10 * (distX - distZ);

        return 14 * distX + 10 * (distZ - distX);
    }

    private bool IsNeighbourWalkable(Node current, Node neighbour, Cell[,] grid)
    {
        return !grid[current.position.x, current.position.y].
            HasWall(ConvertDirToWall(neighbour.position - current.position));

        /*return !grid[neighbour.position.x, neighbour.position.y].
            HasWall(ConvertDirToWall(neighbour.position - current.position));*/
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
    private List<Node> GetCardinalNeighbours(Node node, Cell[,] grid)
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

                // !FIX ??
                neightbours.Add(new Node(new Vector2Int(checkX, checkZ), null, 0, 0));
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
        /// Distance to end node.
        /// </summary>
        public int hCost;

        public Node(Vector2Int position, Node parent, int gCost, int hCost)
        {
            this.position = position;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }
}
