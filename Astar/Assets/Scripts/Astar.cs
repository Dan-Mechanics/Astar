using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
/// https://hku.instructure.com/courses/2268/files/152426?module_item_id=15787
/// https://www.youtube.com/watch?v=Zg0Cxn8AVZA
/// </summary>
public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions.
    /// 
    /// https://youtu.be/mZfyt03LDH4?si=9o3IaTTRF_UvElDl&t=479
    /// </summary>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Node startNode = new Node(startPos, null, 0, 0);
        Node endNode = new Node(endPos, null, 0, 0);

        // make this a queueu?
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // THERE ARE NODES LEFT TO SEARCH.
        while (openSet.Count > 0)
        {
            // FIND CHEAPEST NODE IN OPEN.
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore < currentNode.FScore || openSet[i].FScore == currentNode.FScore && openSet[i].hScore < currentNode.hScore)
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
                return path;
        }

        // THERE IS NO PATH.
        return null;
    }

    /// <summary>
    /// This is the Node class. 
    /// You can use this class to store calculated FScores for the cells of the grid.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// GScore + HScore.
        /// </summary>
        public float FScore => gScore + hScore;

        /// <summary>
        /// Position on the grid.
        /// </summary>
        public Vector2Int position; 

        /// <summary>
        /// Parent Node of this node.
        /// </summary>
        public Node parent; 

        /// <summary>
        /// Current travelled distance.
        /// </summary>
        public float gScore; 
        /// <summary>
        /// Distance estimated based on Heuristic.
        /// </summary>
        public float hScore;

        public Node(Vector2Int position, Node parent, int gScore, int hScore)
        {
            this.position = position;
            this.parent = parent;
            this.gScore = gScore;
            this.hScore = hScore;
        }
    }
}
