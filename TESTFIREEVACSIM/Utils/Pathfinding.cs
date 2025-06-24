using System;
using System.Collections.Generic;
using System.Drawing;
using TESTFIREEVACSIM.Models;

namespace TESTFIREEVACSIM.Utils
{
    public static class Pathfinding
    {
        private class Node
        {
            public Point Position { get; set; }
            public int G { get; set; } // Cost from start
            public int H { get; set; } // Heuristic (estimated cost to goal)
            public int F => G + H; // Total cost
            public Node Parent { get; set; }
            public int Floor { get; set; }

            public Node(Point pos, int floor)
            {
                Position = pos;
                Floor = floor;
            }
        }

        public static List<Point> FindPath(Agent agent, Floor currentFloor, Point target, List<Floor> floors)
        {
            var startPos = new Point((int)agent.X, (int)agent.Y);
            var startNode = new Node(startPos, agent.Floor);
            var targetNode = new Node(target, agent.Floor);

            var openSet = new List<Node> { startNode };
            var closedSet = new HashSet<(int x, int y, int floor)>();
            var nodeDict = new Dictionary<(int x, int y, int floor), Node> 
            {
                { (startPos.X, startPos.Y, agent.Floor), startNode }
            };

            while (openSet.Count > 0)
            {
                // Get node with lowest F cost
                var current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].F < current.F)
                        current = openSet[i];
                }

                if (current.Position == target && current.Floor == agent.Floor)
                {
                    return ReconstructPath(current);
                }

                openSet.Remove(current);
                closedSet.Add((current.Position.X, current.Position.Y, current.Floor));

                foreach (var neighbor in GetNeighbors(current, floors))
                {
                    var neighborTuple = (neighbor.Position.X, neighbor.Position.Y, neighbor.Floor);
                    if (closedSet.Contains(neighborTuple))
                        continue;

                    var tentativeG = current.G + 1;
                    if (!nodeDict.ContainsKey(neighborTuple))
                    {
                        neighbor.G = tentativeG;
                        neighbor.H = CalculateHeuristic(neighbor.Position, target);
                        neighbor.Parent = current;
                        openSet.Add(neighbor);
                        nodeDict[neighborTuple] = neighbor;
                    }
                    else if (tentativeG < nodeDict[neighborTuple].G)
                    {
                        var existingNode = nodeDict[neighborTuple];
                        existingNode.G = tentativeG;
                        existingNode.Parent = current;
                    }
                }
            }

            return null; // No path found
        }

        private static List<Node> GetNeighbors(Node current, List<Floor> floors)
        {
            var neighbors = new List<Node>();
            var floor = floors[current.Floor];

            // Possible movement directions (including diagonals)
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < dx.Length; i++)
            {
                int newX = current.Position.X + dx[i];
                int newY = current.Position.Y + dy[i];

                if (newX >= 0 && newX < floor.Width && 
                    newY >= 0 && newY < floor.Height)
                {
                    var cell = floor.Grid[newX, newY];
                    if (!cell.IsWall)
                    {
                        var neighbor = new Node(new Point(newX, newY), current.Floor);
                        neighbors.Add(neighbor);
                    }
                }
            }

            // Check for stairs to add vertical movement
            var currentCell = floor.Grid[current.Position.X, current.Position.Y];
            if (currentCell.IsStairs)
            {
                // Add connections to stairs on adjacent floors
                if (current.Floor > 0)
                {
                    neighbors.Add(new Node(current.Position, current.Floor - 1));
                }
                if (current.Floor < floors.Count - 1)
                {
                    neighbors.Add(new Node(current.Position, current.Floor + 1));
                }
            }

            return neighbors;
        }

        private static int CalculateHeuristic(Point start, Point end)
        {
            // Using Manhattan distance as heuristic
            return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
        }

        private static List<Point> ReconstructPath(Node endNode)
        {
            var path = new List<Point>();
            var current = endNode;

            while (current != null)
            {
                path.Add(current.Position);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}