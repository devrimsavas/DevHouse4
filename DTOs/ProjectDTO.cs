namespace DevHouse1.DTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; } = string.Empty; // Includes project type name
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty; // Includes team name
    }
}
