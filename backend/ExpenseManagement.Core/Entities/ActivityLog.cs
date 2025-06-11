namespace ExpenseManagement.Core.Entities;

public class ActivityLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int UserId { get; set; }

    // Navigation property
    public virtual User User { get; set; } = null!;
}

public enum ActivityAction
{
    Create,
    Update,
    Delete,
    Login,
    Logout,
    Export,
    Import
}

public enum ActivityEntity
{
    User,
    Expense,
    Category,
    Tag,
    Budget
}
