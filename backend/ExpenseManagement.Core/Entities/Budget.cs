namespace ExpenseManagement.Core.Entities;

public class Budget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BudgetPeriod Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal AlertThreshold { get; set; } = 80; // Percentage
    public bool IsAlertSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public int UserId { get; set; }
    public int? CategoryId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Category? Category { get; set; }
    public virtual ICollection<BudgetAlert> BudgetAlerts { get; set; } = new List<BudgetAlert>();
}

public enum BudgetPeriod
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
    Custom
}

public class BudgetAlert
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public AlertType AlertType { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int BudgetId { get; set; }

    // Navigation property
    public virtual Budget Budget { get; set; } = null!;
}

public enum AlertType
{
    Warning,
    Exceeded,
    NearLimit
}
