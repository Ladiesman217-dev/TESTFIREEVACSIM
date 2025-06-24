using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TESTFIREEVACSIM.Services;

namespace TESTFIREEVACSIM
{
    public partial class MainWindow : Window
    {
        private readonly SimulationService _simulationService;
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _statsTimer;
        private DateTime _simulationStartTime;
        // Add this field

        public MainWindow()
        {
            InitializeComponent();
            
            _simulationStartTime = DateTime.Now;
            _simulationService = new SimulationService(SimulationCanvas);
            _timer = new DispatcherTimer();
            _statsTimer = new DispatcherTimer();
            
            // Set initial floor selection
            FloorComboBox.SelectedIndex = 0;
            
            // Initially disable some buttons
            StartButton.IsEnabled = false;
            TriggerAlarmButton.IsEnabled = false;

            // Initialize stats timer
            _statsTimer.Interval = TimeSpan.FromSeconds(1);
            _statsTimer.Tick += UpdateStatistics;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Ready to initialize simulation");
            UpdateStatistics(null, null);
        }

        private void InitializeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _simulationService.InitializeSimulation();
                StartButton.IsEnabled = true;
                UpdateStatus("Simulation initialized");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing simulation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Initialization failed");
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_simulationService.GetAgentCount() == 0)
                {
                    MessageBox.Show("Please initialize agents before starting the simulation", 
                        "No Agents", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _simulationStartTime = DateTime.Now;
                _simulationService.StartSimulation(_timer);
                _statsTimer.Start();
                
                StartButton.IsEnabled = false;
                TriggerAlarmButton.IsEnabled = true;
                InitializeButton.IsEnabled = false;
                InitializeAgentsButton.IsEnabled = false;
                UpdateStatus("Simulation running");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting simulation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Failed to start simulation");
            }
        }

        private void UpdateStatistics(object sender, EventArgs e)
        {
            try
            {
                // Update simulation time only if simulation is running
                if (_timer.IsEnabled)
                {
                    var elapsedTime = DateTime.Now - _simulationStartTime;
                    SimTimeText.Text = $"Simulation Time: {elapsedTime:mm\\:ss}";
                }

                // Get statistics from simulation service
                var stats = _simulationService.GetSimulationStatistics();
                if (stats != null)
                {
                    AgentCountText.Text = $"Total Agents: {stats.TotalAgents}";
                    EvacuatingText.Text = $"Evacuating: {stats.EvacuatingAgents}";
                    AverageSpeedText.Text = $"Avg. Speed: {stats.AverageSpeed:F2} m/s";
            
                    // Update floor statistics
                    FloorStatsList.Items.Clear();
                    for (int i = 0; i < 3; i++) // For all three floors
                    {
                        int agentCount = stats.AgentsPerFloor.ContainsKey(i) ? stats.AgentsPerFloor[i] : 0;
                        FloorStatsList.Items.Add(new TextBlock 
                        { 
                            Text = i == 0 ? $"Ground: {agentCount}" : $"Floor {i}: {agentCount}"
                        });
                    }

                    DensityText.Text = $"Highest Density: {stats.HighestDensity:F2} agents/m²";
                }
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                UpdateStatus($"Error updating statistics: {ex.Message}");
            }
        }

        private void TriggerAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _simulationService.TriggerAlarm();
                TriggerAlarmButton.IsEnabled = false;
                UpdateStatus("Alarm triggered - evacuation in progress");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error triggering alarm: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FloorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FloorComboBox.SelectedIndex >= 0)
            {
                try
                {
                    _simulationService.SetCurrentFloorIndex(FloorComboBox.SelectedIndex);
                    FloorNumberText.Text = FloorComboBox.SelectedIndex == 0 
                        ? "Current Floor: Ground" 
                        : $"Current Floor: {FloorComboBox.SelectedIndex}";
                    UpdateStatus(FloorComboBox.SelectedIndex == 0 
                        ? "Viewing Ground Floor" 
                        : $"Viewing Floor {FloorComboBox.SelectedIndex}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error changing floors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void InitializeAgentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AgentCountTextBox.Text, out int agentCount))
            {
                if (agentCount <= 0)
                {
                    MessageBox.Show("Please enter a positive number of agents", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    _simulationService.InitializeAgents(agentCount);
                    UpdateStatus($"Initialized {agentCount} agents");
                    StartButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error initializing agents: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number of agents", "Invalid Input", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateStatus(string message)
        {
            StatusBarText.Text = message;
        }
    }

    // Add this class to SimulationService.cs
    public class SimulationStatistics
    {
        public int TotalAgents { get; set; }
        public int EvacuatingAgents { get; set; }
        public double AverageSpeed { get; set; }
        public Dictionary<int, int> AgentsPerFloor { get; set; }
        public double HighestDensity { get; set; }
    }
}