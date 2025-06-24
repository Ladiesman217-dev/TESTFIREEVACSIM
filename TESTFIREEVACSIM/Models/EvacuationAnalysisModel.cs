
using System;
using System.Drawing;
using System.Collections.Generic;

namespace TESTFIREEVACSIM.Models
{
    public class AnalysisModel
    {
        public DateTime TimeStamp { get; set; }
        public int TotalAgents { get; set; }
        public Dictionary<int, int> AgentsPerFloor { get; set; }
        public Dictionary<AgentState, int> AgentStates { get; set; }
        public List<PathAnalysis> PathData { get; set; }
        public double AverageMovementSpeed { get; set; }
        public List<DensityHotspot> DensityHotspots { get; set; }

        public AnalysisModel()
        {
            TimeStamp = DateTime.Now;
            AgentsPerFloor = new Dictionary<int, int>();
            AgentStates = new Dictionary<AgentState, int>();
            PathData = new List<PathAnalysis>();
            DensityHotspots = new List<DensityHotspot>();
        }
    }

    public class PathAnalysis
    {
        public int AgentId { get; set; }
        public List<Point> Path { get; set; }
        public double PathLength { get; set; }
        public double TimeSpent { get; set; }
        public int FloorChanges { get; set; }
    }

    public class DensityHotspot
    {
        public Point Location { get; set; }
        public int Floor { get; set; }
        public int AgentCount { get; set; }
        public double Density { get; set; } // Agents per unit area
    }
}
