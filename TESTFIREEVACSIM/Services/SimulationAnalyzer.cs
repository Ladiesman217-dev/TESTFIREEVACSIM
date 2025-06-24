using System.Collections.Generic;
using TESTFIREEVACSIM.Models;

namespace TESTFIREEVACSIM.Services
{
    public class SimulationAnalyzer
    {
        public SimulationAnalysis AnalyzeSimulationState(List<Agent> agents, List<Floor> floors)
        {
            var analysis = new SimulationAnalysis();

            foreach (var floor in floors)
            {
                var floorIndex = floors.IndexOf(floor);
                var agentsOnFloor = agents.FindAll(a => a.Floor == floorIndex);
                
                analysis.AgentCountByFloor[floorIndex] = agentsOnFloor.Count;
                
                // Calculate density map for the floor
                for (int x = 0; x < floor.Width; x++)
                {
                    for (int y = 0; y < floor.Height; y++)
                    {
                        if (!floor.Grid[x, y].IsWall)
                        {
                            var nearbyAgents = CountNearbyAgents(x, y, agentsOnFloor);
                            if (nearbyAgents > 0)
                            {
                                analysis.DensityPoints.Add(new DensityPoint
                                {
                                    X = x,
                                    Y = y,
                                    Floor = floorIndex,
                                    Density = nearbyAgents
                                });
                            }
                        }
                    }
                }
            }

            return analysis;
        }

        private int CountNearbyAgents(int x, int y, List<Agent> agents)
        {
            const int radius = 2;
            return agents.Count(a =>
                a.X >= x - radius && a.X <= x + radius &&
                a.Y >= y - radius && a.Y <= y + radius);
        }
    }

    public class SimulationAnalysis
    {
        public Dictionary<int, int> AgentCountByFloor { get; set; } = new Dictionary<int, int>();
        public List<DensityPoint> DensityPoints { get; set; } = new List<DensityPoint>();
    }

    public class DensityPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Floor { get; set; }
        public int Density { get; set; }
    }
}