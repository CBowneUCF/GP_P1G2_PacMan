using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;

namespace AStarSystem
{
    public class AGrid : MonoBehaviour
    {

        Node[,] nodeGrid;
        Vector2Int gridSize;

        public readonly bool tileBased = true;
        public Tilemap collisionTileset;
        public Vector2Int bottomLeft;
        public Vector2Int tileDimensions;
        public Vector2 worldOffset;

        public int MaxSize = 300;



        private void Start()
        {
            if (tileBased) CreateGridTileBased();
        }



        void CreateGridTileBased()
        {
            nodeGrid = new Node[tileDimensions.x, tileDimensions.y];

            for (int x = 0; x < tileDimensions.x; x++)
            {
                for (int y = 0; y < tileDimensions.y; y++)
                {
                    Vector2Int point = new Vector2Int(x+bottomLeft.x, y + bottomLeft.y);

                    Tile t = collisionTileset.GetTile<Tile>(new Vector3Int(point.x, point.y, 0));
                    bool walkable = t == null || !(t.colliderType == Tile.ColliderType.Grid);
                    nodeGrid[x, y] = new Node(walkable, point + worldOffset, new Vector2Int(x, y));
                }
            }
        }



        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x - bottomLeft.x - worldOffset.x);
            int y = Mathf.RoundToInt(worldPosition.y - bottomLeft.y - worldOffset.y);
            return nodeGrid[x, y];
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridPosition.x + x;
                    int checkY = node.gridPosition.y + y;

                    if (checkX >= 0 && checkX < tileDimensions.x && checkY >= 0 && checkY < tileDimensions.y)
                    {
                        neighbours.Add(nodeGrid[checkX, checkY]);
                    }
                }
            }
            
            return neighbours;
        }






        public bool FindPath(out Vector2[] output, Vector2 startPos, Vector2 targetPos)
        {

            Vector2[] waypoints = new Vector2[0];
            bool pathSuccess = false;

            Node startNode = NodeFromWorldPoint(startPos);
            Node targetNode = NodeFromWorldPoint(targetPos);
            Debug.LogFormat("{0}, {1}", startNode.worldPosition, targetNode.worldPosition);

            if (startNode.walkable && targetNode.walkable)
            {
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node node = openSet[0];
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
                        pathSuccess = true;
                        break;
                    }
                    

                    foreach (Node neighbour in GetNeighbours(node))
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
            //yield return null;
            if (pathSuccess)
            {
                Debug.Log(1);
                waypoints = RetracePath(startNode, targetNode);
            }
            //requestManager.FinishedProcessingPath(waypoints, pathSuccess);
            output = waypoints;
            Debug.Log(waypoints[0]);
            return pathSuccess;
        }

        Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector2[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;

        }

        Vector2[] SimplifyPath(List<Node> path)
        {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].gridPosition.x - path[i].gridPosition.x, path[i - 1].gridPosition.y - path[i].gridPosition.y);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }


        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }




















        public List<Node> path;
        private void OnDrawGizmos()
        {
            if (false) return;

            //Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, worldSize.y, 1));

            if (nodeGrid != null)
            {
                foreach (Node n in nodeGrid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (1 - .1f) - (Vector3.forward/4));
                }
            }
        }


        public class Node : IHeapItem<Node>
        {

            public bool walkable;
            public Vector2 worldPosition;
            public Vector2Int gridPosition;

            public int gCost;
            public int hCost;
            public Node parent;
            int heapIndex;

            public Node(bool _walkable, Vector2 _worldPos, Vector2Int _gridPos)
            {
                walkable = _walkable;
                worldPosition = _worldPos;
                gridPosition = _gridPos;
            }

            public int fCost
            {
                get
                {
                    return gCost + hCost;
                }
            }

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

            public int CompareTo(Node nodeToCompare)
            {
                int compare = fCost.CompareTo(nodeToCompare.fCost);
                if (compare == 0)
                {
                    compare = hCost.CompareTo(nodeToCompare.hCost);
                }
                return -compare;
            }
        }

    }

    

}

