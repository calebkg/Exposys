namespace ExpenseManagement.Core.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#10B981";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ExpenseTag> ExpenseTags { get; set; } = new List<ExpenseTag>();
}

public class ExpenseTag
{
    public int ExpenseId { get; set; }
    public int TagId { get; set; }

    // Navigation properties
    public virtual Expense Expense { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
