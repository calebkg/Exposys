using ExpenseManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ExpenseTag> ExpenseTags { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetAlert> BudgetAlerts { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure User
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProfilePicture).HasMaxLength(500);
        });

        // Configure Expense
        builder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.ReceiptUrl).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CategoryId);
        });

        // Configure Category
        builder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Color).HasMaxLength(7).IsRequired(); // #FFFFFF format
            entity.Property(c => c.Icon).HasMaxLength(50).IsRequired();

            entity.HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
        });

        // Configure Tag
        builder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(50).IsRequired();
            entity.Property(t => t.Color).HasMaxLength(7).IsRequired(); // #FFFFFF format

            entity.HasOne(t => t.User)
                .WithMany(u => u.Tags)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => new { t.UserId, t.Name }).IsUnique();
        });

        // Configure ExpenseTag (Many-to-Many)
        builder.Entity<ExpenseTag>(entity =>
        {
            entity.HasKey(et => new { et.ExpenseId, et.TagId });

            entity.HasOne(et => et.Expense)
                .WithMany(e => e.ExpenseTags)
                .HasForeignKey(et => et.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(et => et.Tag)
                .WithMany(t => t.ExpenseTags)
                .HasForeignKey(et => et.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Budget
        builder.Entity<Budget>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).HasMaxLength(200).IsRequired();
            entity.Property(b => b.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(b => b.AlertThreshold).HasColumnType("decimal(5,2)").IsRequired();
            entity.Property(b => b.Period).HasConversion<string>();

            entity.HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(b => b.UserId);
            entity.HasIndex(b => new { b.StartDate, b.EndDate });
        });

        // Configure BudgetAlert
        builder.Entity<BudgetAlert>(entity =>
        {
            entity.HasKey(ba => ba.Id);
            entity.Property(ba => ba.Message).HasMaxLength(500).IsRequired();
            entity.Property(ba => ba.AlertType).HasConversion<string>();

            entity.HasOne(ba => ba.Budget)
                .WithMany(b => b.BudgetAlerts)
                .HasForeignKey(ba => ba.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ba => ba.BudgetId);
            entity.HasIndex(ba => ba.CreatedAt);
        });

        // Configure ActivityLog
        builder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(al => al.Id);
            entity.Property(al => al.Action).HasMaxLength(50).IsRequired();
            entity.Property(al => al.Entity).HasMaxLength(50).IsRequired();
            entity.Property(al => al.IpAddress).HasMaxLength(45);
            entity.Property(al => al.UserAgent).HasMaxLength(500);

            entity.HasOne(al => al.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(al => al.UserId);
            entity.HasIndex(al => al.Timestamp);
            entity.HasIndex(al => new { al.Entity, al.EntityId });
        });

        // Seed default roles
        builder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
            new Role { Id = 2, Name = "User", NormalizedName = "USER" }
        );
    }
}
