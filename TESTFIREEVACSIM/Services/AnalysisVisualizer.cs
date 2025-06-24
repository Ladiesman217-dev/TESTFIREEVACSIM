using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TESTFIREEVACSIM.Services
{
    public class AnalysisVisualizer
    {
        private readonly Canvas _canvas;
        private const int CELL_SIZE = 20;

        public AnalysisVisualizer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void VisualizeAnalysis(SimulationAnalysis analysis, int currentFloor)
        {
            // Visualize density points for current floor
            foreach (var densityPoint in analysis.DensityPoints)
            {
                if (densityPoint.Floor == currentFloor)
                {
                    DrawDensityPoint(densityPoint);
                }
            }
        }

        private void DrawDensityPoint(DensityPoint point)
        {
            double opacity = CalculateOpacity(point.Density);
            var densityMarker = new Rectangle
            {
                Width = CELL_SIZE,
                Height = CELL_SIZE,
                Fill = new SolidColorBrush(Colors.Red) { Opacity = opacity },
                Opacity = 0.3
            };

            Canvas.SetLeft(densityMarker, point.X * CELL_SIZE);
            Canvas.SetTop(densityMarker, point.Y * CELL_SIZE);
            Canvas.SetZIndex(densityMarker, -1); // Place behind other elements

            _canvas.Children.Add(densityMarker);
        }

        private static double CalculateOpacity(int density)
        {
            // Convert density to opacity value between 0.1 and 0.8
            const double maxDensity = 10.0;
            return 0.1 + (0.7 * (density / maxDensity));
        }
    }
}