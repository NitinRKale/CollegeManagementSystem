namespace CollegeWebApplication.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string KeyValues { get; set; } = string.Empty; // JSON
        public string OldValues { get; set; } = string.Empty; // JSON
        public string NewValues { get; set; } = string.Empty; // JSON
        public string Action { get; set; } = string.Empty; // Insert/Update/Delete
        public string? UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
