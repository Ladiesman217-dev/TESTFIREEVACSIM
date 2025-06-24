namespace TESTFIREEVACSIM.Models
{
    public struct Sensor
    {
        public int X { get; set; } // X coordinate in the grid
        public int Y { get; set; } // Y coordinate in the grid
        public int Floor { get; set; }
        public SensorType Type { get; set; } // Type of sensor (e.g., smoke, heat, motion)
        public bool Triggered { get; set; } // Indicates if the sensor has been triggered
        public double Range { get; set; } // the range of the sensor
        // Add other relevant attributes (e.g., sensitivity, detection range)
    }
}