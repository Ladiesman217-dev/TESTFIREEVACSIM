using System;
using System.Collections.Generic;
using System.Drawing;

namespace TESTFIREEVACSIM.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Floor { get; set; }
        public double Speed { get; set; }
        public AgentState State { get; set; }
        public DateTime LastMoveTime { get; set; }
        public bool IsEvacuating { get; set; }

        private List<Point> _path;
        private int _currentPathIndex;

        public bool HasPath => _path != null && _currentPathIndex < _path.Count;

        public Point? GetNextPoint()
        {
            if (!HasPath) return null;
            return _path[_currentPathIndex];
        }

        public void AdvanceToNextPathPoint()
        {
            if (HasPath)
            {
                _currentPathIndex++;
                if (_currentPathIndex >= _path.Count)
                {
                    _path = null;
                    _currentPathIndex = 0;
                }
            }
        }

        public void SetNewPath(List<Point> path)
        {
            _path = path;
            _currentPathIndex = 0;
        }

        public void MoveTo(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}