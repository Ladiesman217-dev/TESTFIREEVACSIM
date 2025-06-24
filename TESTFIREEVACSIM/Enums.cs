namespace TESTFIREEVACSIM
{
    // Enums
    public enum AgentState
    {
        Wandering,
        Evacuating,
        Idle,
        Injured,
    }

    public enum SignType
    {
        ExitDirection, // Points towards an exit
        NoElevator // Indicates not to use elevators
        // Add other sign types as needed
    }

    public enum SensorType
    {
        Smoke,
        Heat,
        Motion
    }
}