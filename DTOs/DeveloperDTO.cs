namespace DevHouse1.DTOs
{
    public class DeveloperDTO
    {
        public int Id { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty; // Includes role name
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty; // Includes team name
    }
}
