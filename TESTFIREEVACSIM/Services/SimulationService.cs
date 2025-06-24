using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using TESTFIREEVACSIM.Models;
using TESTFIREEVACSIM.Utils;
using System.Drawing; // Required for Canvas
namespace TESTFIREEVACSIM.Services
{
    public class SimulationService
    {
        private readonly List<Agent> _agents = new List<Agent>();
        private readonly List<Floor> _floors = new List<Floor>();
        private readonly Random _random = new Random();
        private int _currentFloorIndex = 0;
        private readonly Renderer _renderer;
        private bool _isAlarmActive = false;
        private const double AGENT_MOVE_CHANCE = 0.8; // 80% chance to move each tick
        private readonly SimulationAnalyzer _analyzer;
        private readonly AnalysisVisualizer _analysisVisualizer;
        private DateTime _simulationStartTime;

        public SimulationService(Canvas canvas)
        {
            _renderer = new Renderer(canvas);
            _analyzer = new SimulationAnalyzer();
            _analysisVisualizer = new AnalysisVisualizer(canvas);
            _simulationStartTime = DateTime.Now;
        }

        public void InitializeSimulation()
        {
            InitializeFloors();
            // Remove automatic agent initialization
            if (_renderer != null)
            {
                _renderer.RenderScene(_floors, _agents, _currentFloorIndex);
            }
        }

        private void InitializeFloors()
        {
            // Ground Floor (Floor 0)
            var groundFloor = new Floor(50, 40);
            groundFloor.InitializeGrid();

            // Create main outer walls
            groundFloor.CreateRoom(0, 0, 50, 40);

            // Main entrance hall
            groundFloor.CreateRoom(20, 0, 10, 10);
            groundFloor.CreateDoor(24, 0);  // Main entrance
            groundFloor.CreateDoor(24, 10); // Door to corridor

            // Main corridor
            groundFloor.CreateCorridor(5, 20, 40, true);
            
            // Reception area
            groundFloor.CreateRoom(15, 5, 20, 12);
            groundFloor.CreateDoor(24, 17); // Reception to corridor

            // Left wing - Security and storage
            groundFloor.CreateRoom(5, 5, 8, 12);    // Security office
            groundFloor.CreateRoom(5, 25, 15, 12);  // Storage
            groundFloor.CreateDoor(8, 17);          // Security door
            groundFloor.CreateDoor(12, 25);         // Storage door
            groundFloor.CreateDoor(5, 8);           // Emergency exit left

            // Right wing - Cafeteria and facilities
            groundFloor.CreateRoom(35, 5, 10, 12);  // Cafeteria
            groundFloor.CreateRoom(30, 25, 15, 12); // Facilities
            groundFloor.CreateDoor(37, 17);         // Cafeteria door
            groundFloor.CreateDoor(37, 25);         // Facilities door
            groundFloor.CreateDoor(44, 8);          // Emergency exit right

            // Add staircases
            groundFloor.AddStairs(new Point(25, 5));  // Main staircase
            groundFloor.AddStairs(new Point(25, 35)); // Emergency staircase

            // Additional emergency exits
            groundFloor.CreateDoor(0, 20);   // West emergency exit
            groundFloor.CreateDoor(49, 20);  // East emergency exit
            groundFloor.CreateDoor(25, 39);  // South emergency exit

            groundFloor.UpdatePossiblePoints();
            _floors.Add(groundFloor);

            // First Floor (Floor 1)
            var floor1 = new Floor(50, 40);
            floor1.InitializeGrid();
            floor1.CreateRoom(0, 0, 50, 40);

            // Main corridor with cross-section
            floor1.CreateCorridor(5, 20, 40, true);
            floor1.CreateCorridor(25, 5, 30, false);

            // Office spaces
            floor1.CreateRoom(5, 5, 15, 12);    // Office block 1
            floor1.CreateRoom(30, 5, 15, 12);   // Office block 2
            floor1.CreateRoom(5, 25, 15, 12);   // Office block 3
            floor1.CreateRoom(30, 25, 15, 12);  // Office block 4

            // Add doors to offices
            floor1.CreateDoor(12, 17);  // Office block 1
            floor1.CreateDoor(37, 17);  // Office block 2
            floor1.CreateDoor(12, 25);  // Office block 3
            floor1.CreateDoor(37, 25);  // Office block 4

            // Add staircases in the same positions
            floor1.AddStairs(new Point(25, 5));
            floor1.AddStairs(new Point(25, 35));

            floor1.UpdatePossiblePoints();
            _floors.Add(floor1);

            // Second Floor (Floor 2)
            var floor2 = new Floor(50, 40);
            floor2.InitializeGrid();
            floor2.CreateRoom(0, 0, 50, 40);

            // Conference area
            floor2.CreateRoom(15, 5, 20, 15);   // Main conference room
            floor2.CreateDoor(25, 20);          // Conference room door

            // Office spaces
            floor2.CreateRoom(5, 25, 15, 12);   // Executive offices
            floor2.CreateRoom(30, 25, 15, 12);  // Meeting rooms
            floor2.CreateDoor(12, 25);          // Executive office door
            floor2.CreateDoor(37, 25);          // Meeting rooms door

            // Small meeting rooms
            floor2.CreateRoom(5, 5, 8, 8);      // Meeting room 1
            floor2.CreateRoom(38, 5, 8, 8);     // Meeting room 2
            floor2.CreateDoor(9, 13);           // Meeting room 1 door
            floor2.CreateDoor(42, 13);          // Meeting room 2 door

            // Add staircases
            floor2.AddStairs(new Point(25, 5));
            floor2.AddStairs(new Point(25, 35));

            floor2.UpdatePossiblePoints();
            _floors.Add(floor2);

            // Add connecting corridors and additional doors
            foreach (var floor in _floors)
            {
                AddConnectingCorridors(floor);
                AddAdditionalDoors(floor);
                floor.UpdatePossiblePoints();
            }
        }

        private void AddConnectingCorridors(Floor floor)
        {
            // Add horizontal connecting corridors
            floor.CreateCorridor(15, 20, 20, true);
            
            // Add vertical connecting corridors
            floor.CreateCorridor(25, 10, 20, false);
        }

        private void AddAdditionalDoors(Floor floor)
        {
            for (int x = 1; x < floor.Width - 1; x++)
            {
                for (int y = 1; y < floor.Height - 1; y++)
                {
                    if (floor.Grid[x, y].IsWall)
                    {
                        bool hasVerticalSpaces = !floor.Grid[x, y - 1].IsWall && !floor.Grid[x, y + 1].IsWall;
                        bool hasHorizontalSpaces = !floor.Grid[x - 1, y].IsWall && !floor.Grid[x + 1, y].IsWall;

                        if ((hasVerticalSpaces || hasHorizontalSpaces) && _random.Next(100) < 15)
                        {
                            floor.CreateDoor(x, y);
                        }
                    }
                }
            }
        }

        public void InitializeAgents(int count)
        {
            // Clear existing agents
            _agents.Clear();

            for (int i = 0; i < count; i++)
            {
                var randomFloorIndex = _random.Next(_floors.Count);
                var floor = _floors[randomFloorIndex];
                var position = GetRandomEmptyPosition(floor);

                if (position.HasValue)
                {
                    var agent = new Agent
                    {
                        Id = i,
                        X = position.Value.X,
                        Y = position.Value.Y,
                        Floor = randomFloorIndex,
                        Speed = _random.NextDouble() * 0.5 + 0.75, // Speed between 0.75 and 1.25
                        IsEvacuating = false,
                        State = AgentState.Idle,
                        LastMoveTime = DateTime.Now
                    };
                    _agents.Add(agent);
                }
            }

            // Update the visualization if renderer exists
            if (_renderer != null)
            {
                _renderer.RenderScene(_floors, _agents, _currentFloorIndex);
            }
        }

        private Point? GetRandomEmptyPosition(Floor floor)
        {
            if (floor.PossiblePoints.Count == 0)
                return null;

            return floor.PossiblePoints[_random.Next(floor.PossiblePoints.Count)];
        }

        private void UpdateSimulation()
        {
            var currentTime = DateTime.Now;
            foreach (var agent in _agents)
            {
                // Check if enough time has passed based on agent's speed
                var timeSinceLastMove = (currentTime - agent.LastMoveTime).TotalMilliseconds;
                var moveInterval = 500 / agent.Speed; // Base interval modified by speed

                if (timeSinceLastMove >= moveInterval && _random.NextDouble() < AGENT_MOVE_CHANCE)
                {
                    UpdateAgent(agent);
                    agent.LastMoveTime = currentTime;
                }
            }
            
            _renderer.RenderScene(_floors, _agents, _currentFloorIndex);

            // Add analysis
            var analysis = _analyzer.AnalyzeSimulationState(_agents, _floors);
            _analysisVisualizer.VisualizeAnalysis(analysis, _currentFloorIndex);
        }

        private void UpdateAgent(Agent agent)
        {
            if (!agent.HasPath || _random.NextDouble() < 0.1)
            {
                AssignNewTarget(agent);
                return;
            }

            var nextPoint = agent.GetNextPoint();
            if (nextPoint.HasValue)
            {
                // Check if the current position is stairs
                bool isCurrentStairs = _floors[agent.Floor].Grid[(int)agent.X, (int)agent.Y].IsStairs;
                bool isNextStairs = _floors[agent.Floor].Grid[nextPoint.Value.X, nextPoint.Value.Y].IsStairs;

                if (isCurrentStairs && isNextStairs)
                {
                    // Change floor when moving between stairs
                    if (agent.Floor < _floors.Count - 1 && nextPoint.Value.Y > agent.Y)
                    {
                        agent.Floor++;
                    }
                    else if (agent.Floor > 0 && nextPoint.Value.Y < agent.Y)
                    {
                        agent.Floor--;
                    }
                }

                // Check if the next point is a valid move
                if (IsValidMove(nextPoint.Value, agent.Floor))
                {
                    agent.MoveTo(nextPoint.Value.X, nextPoint.Value.Y);
                    
                    if (!agent.HasPath)
                    {
                        AssignNewTarget(agent);
                    }
                }
                else
                {
                    AssignNewTarget(agent);
                }
            }
        }

        private bool IsValidMove(Point point, int floor)
        {
            // Only check if the point is within bounds and not a wall
            return point.X >= 0 && point.X < _floors[floor].Width &&
                   point.Y >= 0 && point.Y < _floors[floor].Height &&
                   !_floors[floor].Grid[point.X, point.Y].IsWall;
        }

        private void AssignNewTarget(Agent agent)
        {
            // Randomly choose a target floor
            int targetFloor = _random.Next(_floors.Count);
            Floor targetFloorObj = _floors[targetFloor];
            var validPoints = GetValidDestinations(targetFloorObj);

            if (validPoints.Count == 0) return;

            int maxAttempts = 5;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Point target = validPoints[_random.Next(validPoints.Count)];
                var path = FindPathBetweenFloors(agent, target, targetFloor);
                
                if (path != null && path.Count > 0)
                {
                    agent.SetNewPath(path);
                    return;
                }
            }
        }

        private List<Point> FindPathBetweenFloors(Agent agent, Point target, int targetFloor)
        {
            // If on the same floor, use regular pathfinding
            if (agent.Floor == targetFloor)
            {
                return Pathfinding.FindPath(agent, _floors[agent.Floor], target, _floors);
            }

            // Find nearest stairs on current floor
            var startPos = new Point((int)agent.X, (int)agent.Y);
            var nearestStairs = FindNearestStairs(_floors[agent.Floor], startPos);
            
            if (nearestStairs == null) return null;

            // Find nearest stairs on target floor
            var targetStairs = FindNearestStairs(_floors[targetFloor], target);
            if (targetStairs == null) return null;

            // Get path to stairs on current floor
            var pathToStairs = Pathfinding.FindPath(agent, _floors[agent.Floor], nearestStairs.Value, _floors);
            if (pathToStairs == null) return null;

            // Create a temporary agent at the stairs position on the target floor
            var tempAgent = new Agent { X = targetStairs.Value.X, Y = targetStairs.Value.Y, Floor = targetFloor };
            
            // Get path from stairs to target on target floor
            var pathFromStairs = Pathfinding.FindPath(tempAgent, _floors[targetFloor], target, _floors);
            if (pathFromStairs == null) return null;

            // Combine paths
            var completePath = new List<Point>();
            completePath.AddRange(pathToStairs);
            completePath.AddRange(pathFromStairs);
            
            return completePath;
        }

        private Point? FindNearestStairs(Floor floor, Point position)
        {
            Point? nearest = null;
            double minDistance = double.MaxValue;

            for (int x = 0; x < floor.Width; x++)
            {
                for (int y = 0; y < floor.Height; y++)
                {
                    if (floor.Grid[x, y].IsStairs)
                    {
                        double distance = Math.Sqrt(
                            Math.Pow(position.X - x, 2) + 
                            Math.Pow(position.Y - y, 2)
                        );
                        
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearest = new Point(x, y);
                        }
                    }
                }
            }

            return nearest;
        }

        private List<Point> GetValidDestinations(Floor floor)
        {
            var validPoints = new List<Point>();
            
            for (int x = 0; x < floor.Width; x++)
            {
                for (int y = 0; y < floor.Height; y++)
                {
                    if (!floor.Grid[x, y].IsWall)
                    {
                        validPoints.Add(new Point(x, y));
                    }
                }
            }
            
            return validPoints;
        }

        public void StartSimulation(DispatcherTimer timer)
        {
            _simulationStartTime = DateTime.Now;
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += (s, e) => UpdateSimulation();
            timer.Start();
        }

        public void SetCurrentFloorIndex(int index)
        {
            if (index >= 0 && index < _floors.Count)
            {
                _currentFloorIndex = index;
                if (_renderer != null)
                {
                    _renderer.RenderScene(_floors, _agents, _currentFloorIndex);
                }
            }
            else
            {
                throw new ArgumentException("Invalid floor index");
            }
        }
        // ... (keep your existing SetCurrentFloorIndex and TriggerAlarmButton methods) ...

        public int GetAgentCount()
        {
            return _agents.Count;
        }

        public SimulationStatistics GetSimulationStatistics()
        {
            var stats = new SimulationStatistics
            {
                TotalAgents = _agents.Count,
                EvacuatingAgents = _agents.Count(a => a.State == AgentState.Evacuating),
                AverageSpeed = _agents.Any() ? _agents.Average(a => a.Speed) : 0,
                AgentsPerFloor = new Dictionary<int, int>(),
                HighestDensity = CalculateHighestDensity()
            };

            // Calculate agents per floor
            foreach (var agent in _agents)
            {
                if (!stats.AgentsPerFloor.ContainsKey(agent.Floor))
                    stats.AgentsPerFloor[agent.Floor] = 0;
                stats.AgentsPerFloor[agent.Floor]++;
            }

            return stats;
        }

        private double CalculateHighestDensity()
        {
            const int gridSize = 5; // Size of area to check for density
            double highestDensity = 0;

            foreach (var floor in _floors)
            {
                for (int x = 0; x < floor.Width - gridSize; x++)
                {
                    for (int y = 0; y < floor.Height - gridSize; y++)
                    {
                        // Only count walkable areas
                        if (floor.Grid[x, y].IsWall) continue;

                        var agentsInArea = _agents.Count(a =>
                            a.Floor == _floors.IndexOf(floor) &&
                            a.X >= x && a.X < x + gridSize &&
                            a.Y >= y && a.Y < y + gridSize);

                        double density = agentsInArea / (double)(gridSize * gridSize);
                        highestDensity = Math.Max(highestDensity, density);
                    }
                }
            }

            return highestDensity;
        }

        public void TriggerAlarm()
        {
            _isAlarmActive = true;
            foreach (var agent in _agents)
            {
                agent.State = AgentState.Evacuating;
            }
        }
    }
}

public class SimulationStatistics
{
    public int TotalAgents { get; set; }
    public int EvacuatingAgents { get; set; }
    public double AverageSpeed { get; set; }
    public Dictionary<int, int> AgentsPerFloor { get; set; }
    public double HighestDensity { get; set; }
}