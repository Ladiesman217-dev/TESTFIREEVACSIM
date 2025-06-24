using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TESTFIREEVACSIM.Models;

namespace TESTFIREEVACSIM
{
    public class Renderer
    {
        private readonly Canvas _canvas;
        private const int CELL_SIZE = 20; // Size of each grid cell in pixels
        private const int AGENT_SIZE = 12; // Size of agent circles
        private const double WALL_THICKNESS = 2.0;

        public Renderer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void RenderScene(List<Floor> floors, List<Agent> agents, int currentFloorIndex)
        {
            _canvas.Children.Clear();

            if (currentFloorIndex < 0 || currentFloorIndex >= floors.Count)
                return;

            var currentFloor = floors[currentFloorIndex];

            // Calculate scaling to fit the floor in the canvas
            double scaleX = _canvas.ActualWidth / (currentFloor.Width * CELL_SIZE);
            double scaleY = _canvas.ActualHeight / (currentFloor.Height * CELL_SIZE);
            double scale = Math.Min(scaleX, scaleY);

            // Draw grid and walls
            DrawGrid(currentFloor, scale);

            // Draw agents on current floor
            foreach (var agent in agents)
            {
                if (agent.Floor == currentFloorIndex)
                {
                    DrawAgent(agent, scale);
                }
            }
        }

        private void DrawGrid(Floor floor, double scale)
        {
            for (int x = 0; x < floor.Width; x++)
            {
                for (int y = 0; y < floor.Height; y++)
                {
                    var cell = floor.Grid[x, y];
                    double scaledX = x * CELL_SIZE * scale;
                    double scaledY = y * CELL_SIZE * scale;
                    double scaledSize = CELL_SIZE * scale;

                    if (cell.IsWall)
                    {
                        var wall = new Rectangle
                        {
                            Width = scaledSize,
                            Height = scaledSize,
                            Fill = Brushes.DarkGray
                        };
                        Canvas.SetLeft(wall, scaledX);
                        Canvas.SetTop(wall, scaledY);
                        _canvas.Children.Add(wall);
                    }
                    else if (cell.IsDoor)
                    {
                        var door = new Rectangle
                        {
                            Width = scaledSize,
                            Height = scaledSize,
                            Fill = Brushes.Green,
                            Opacity = 0.5
                        };
                        Canvas.SetLeft(door, scaledX);
                        Canvas.SetTop(door, scaledY);
                        _canvas.Children.Add(door);
                    }
                    else if (cell.IsStairs)
                    {
                        var stairs = new Rectangle
                        {
                            Width = scaledSize,
                            Height = scaledSize,
                            Fill = Brushes.Orange,
                            Opacity = 0.5
                        };
                        Canvas.SetLeft(stairs, scaledX);
                        Canvas.SetTop(stairs, scaledY);
                        _canvas.Children.Add(stairs);
                    }

                    // Draw grid lines
                    var gridLine = new Rectangle
                    {
                        Width = scaledSize,
                        Height = scaledSize,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 0.5,
                        Fill = Brushes.Transparent
                    };
                    Canvas.SetLeft(gridLine, scaledX);
                    Canvas.SetTop(gridLine, scaledY);
                    _canvas.Children.Add(gridLine);
                }
            }
        }

        private void DrawAgent(Agent agent, double scale)
        {
            double scaledX = (agent.X * CELL_SIZE + CELL_SIZE / 2) * scale;
            double scaledY = (agent.Y * CELL_SIZE + CELL_SIZE / 2) * scale;
            double scaledSize = AGENT_SIZE * scale;

            var agentCircle = new Ellipse
            {
                Width = scaledSize,
                Height = scaledSize,
                Fill = GetAgentBrush(agent.State),
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(agentCircle, scaledX - scaledSize / 2);
            Canvas.SetTop(agentCircle, scaledY - scaledSize / 2);
            _canvas.Children.Add(agentCircle);
        }

        private static Brush GetAgentBrush(AgentState state)
        {
            return state switch
            {
                AgentState.Idle => Brushes.Blue,
                AgentState.Wandering => Brushes.Green,
                AgentState.Evacuating => Brushes.Red,
                AgentState.Injured => Brushes.Purple,
                _ => Brushes.Black
            };
        }
    }
}