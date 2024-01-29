using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStarSystem
{
    public class APathFinding : MonoBehaviour
    {
        public Transform seeker, target;
        AGrid grid;

        void Awake()
        {
            grid = GetComponent<AGrid>();
        }

        void Update()
        {
            FindPath(seeker.position, target.position);
        }

        void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            AGrid.Node startNode = grid.NodeFromWorldPoint(startPos);
            AGrid.Node targetNode = grid.NodeFromWorldPoint(targetPos);

            List<AGrid.Node> openSet = new List<AGrid.Node>();
            HashSet<AGrid.Node> closedSet = new HashSet<AGrid.Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                AGrid.Node node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        if (openSet[i].hCost < node.hCost)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (AGrid.Node neighbour in grid.GetNeighbours(node))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        void RetracePath(AGrid.Node startNode, AGrid.Node endNode)
        {
            List<AGrid.Node> path = new List<AGrid.Node>();
            AGrid.Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            grid.path = path;

        }

        int GetDistance(AGrid.Node nodeA, AGrid.Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}

