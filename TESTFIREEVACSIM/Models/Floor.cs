using System.Collections.Generic;
using System.Drawing;

namespace TESTFIREEVACSIM.Models
{
    public class Floor
    {
        public int Width { get; }
        public int Height { get; }
        public Cell[,] Grid { get; private set; }
        public List<Point> PossiblePoints { get; private set; }

        public Floor(int width, int height)
        {
            Width = width;
            Height = height;
            PossiblePoints = new List<Point>();
            InitializeGrid();
        }

        public void InitializeGrid()
        {
            Grid = new Cell[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Grid[x, y] = new Cell();
                }
            }
        }

        public void CreateRoom(int x, int y, int width, int height)
        {
            for (int i = x; i < x + width && i < Width; i++)
            {
                for (int j = y; j < y + height && j < Height; j++)
                {
                    // Create walls at the boundaries
                    if (i == x || i == x + width - 1 || j == y || j == y + height - 1)
                    {
                        Grid[i, j].IsWall = true;
                    }
                }
            }
            UpdatePossiblePoints();
        }

        public void CreateCorridor(int x, int y, int length, bool horizontal)
        {
            if (horizontal)
            {
                for (int i = x; i < x + length && i < Width; i++)
                {
                    if (y - 1 >= 0) Grid[i, y - 1].IsWall = true;
                    if (y + 1 < Height) Grid[i, y + 1].IsWall = true;
                }
            }
            else
            {
                for (int j = y; j < y + length && j < Height; j++)
                {
                    if (x - 1 >= 0) Grid[x - 1, j].IsWall = true;
                    if (x + 1 < Width) Grid[x + 1, j].IsWall = true;
                }
            }
            UpdatePossiblePoints();
        }

        public void CreateDoor(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Grid[x, y].IsWall = false;
                Grid[x, y].IsDoor = true;
            }
            UpdatePossiblePoints();
        }

        public void AddStairs(Point location)
        {
            if (location.X >= 0 && location.X < Width && 
                location.Y >= 0 && location.Y < Height)
            {
                Grid[location.X, location.Y].IsStairs = true;
            }
            UpdatePossiblePoints();
        }

        public void UpdatePossiblePoints()
        {
            PossiblePoints.Clear();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!Grid[x, y].IsWall && !Grid[x, y].IsStairs)
                    {
                        PossiblePoints.Add(new Point(x, y));
                    }
                }
            }
        }
    }
}