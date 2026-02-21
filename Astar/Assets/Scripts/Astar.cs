using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
/// </summary>
public class Astar
{
    private Node[,] grid;
    
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] cells)
    {
        int gridWidth = cells.GetLength(0);
        int gridHeight = cells.GetLength(1);
        if (grid == null || grid.GetLength(0) != gridWidth || grid.GetLength(1) != gridHeight)
        {
            grid = new Node[gridWidth, gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y] = new Node(new Vector2Int(x, y), null, 0, 0);
                }
            }
        }

        if (startPos.x < 0 || startPos.x >= gridWidth)
            return null;

        if (endPos.x < 0 || endPos.x >= gridWidth)
            return null;

        if (startPos.y < 0 || startPos.y >= gridHeight)
            return null;

        if (endPos.y < 0 || endPos.y >= gridHeight)
            return null;

        Node startNode = grid[startPos.x, startPos.y];
        Node endNode = grid[endPos.x, endPos.y];

        Heap<Node> openSet = new Heap<Node>(gridWidth * gridHeight);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet.RemoveFirst();
            closedSet.Add(node);

            if (node == endNode)
                return RetracePath(startNode, node);

            foreach (Node neighbour in GetNeighbours(node, grid, gridWidth, gridHeight))
            {
                if (!IsNeighbourWalkable(node, neighbour, cells) || closedSet.Contains(neighbour))
                    continue;

                int newNeighbourGCost = node.gCost + GetDistanceBetweenNodes(node, neighbour);
                if (newNeighbourGCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newNeighbourGCost;
                    neighbour.hCost = GetDistanceBetweenNodes(neighbour, endNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode) 
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node node = endNode;

        while (node != startNode)
        {
            path.Add(node.position);
            node = node.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistanceBetweenNodes(Node a, Node b)
    {
        int distX = Mathf.Abs(a.position.x - b.position.x);
        int distY = Mathf.Abs(a.position.y - b.position.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }

    private bool IsNeighbourWalkable(Node node, Node neighbour, Cell[,] cells)
    {
        int x = neighbour.position.x - node.position.x;
        int y = neighbour.position.y - node.position.y;

        // CARDINAL.
        Cell cell = cells[node.position.x, node.position.y];
        if (x > 0 && cell.HasWall(Wall.RIGHT))
            return false;

        if (x < 0 && cell.HasWall(Wall.LEFT))
            return false;

        if (y > 0 && cell.HasWall(Wall.UP))
            return false;

        if (y < 0 && cell.HasWall(Wall.DOWN))
            return false;

        // DIAGONAL.
        if (Mathf.Abs(x) == Mathf.Abs(y))
        {
            Cell verticalCell = cells[node.position.x, node.position.y + y];
            Cell horizontalCell = cells[node.position.x + x, node.position.y];

            if (x > 0 && verticalCell.HasWall(Wall.RIGHT))
                return false;

            if (x < 0 && verticalCell.HasWall(Wall.LEFT))
                return false;

            if (y > 0 && horizontalCell.HasWall(Wall.UP))
                return false;

            if (y < 0 && horizontalCell.HasWall(Wall.DOWN))
                return false;
        }

        return true;
    }

    private List<Node> GetNeighbours(Node node, Node[,] grid, int gridWidth, int gridHeight)
    {
        List<Node> neightbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.position.x + x;
                int checkY = node.position.y + y;

                // CHECK GRID BOUNDS.
                if(checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                    neightbours.Add(grid[checkX, checkY]);
            }
        }

        return neightbours;
    }

    public class Node : IHeapItem<Node>
    {
        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        private int heapIndex;

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
        /// Guesstimate distance to end node.
        /// </summary>
        public int hCost;

        public Node(Vector2Int position, Node parent, int gCost, int hCost)
        {
            this.position = position;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public int CompareTo(Node other)
        {
            int result = FCost.CompareTo(other.FCost);
            if (result == 0)
                result = hCost.CompareTo(other.hCost);

            return -result;
        }

        public override string ToString() => position.ToString();
    }
}
