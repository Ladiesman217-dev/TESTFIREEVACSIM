namespace TESTFIREEVACSIM.Models
{
    public struct Wall
    {
        public bool IsWall { get; set; }
        public bool Sign { get; set; }
        public SignType SignType { get; set; }
        public string SignDirection { get; set; }
        public bool IsStair { get; set; } // Add this property
    }
}