using ExpenseManagement.Core.Interfaces;
using ExpenseManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpenseManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IExpenseRepository? _expenses;
    private ICategoryRepository? _categories;
    private ITagRepository? _tags;
    private IBudgetRepository? _budgets;
    private IActivityLogRepository? _activityLogs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IExpenseRepository Expenses => 
        _expenses ??= new ExpenseRepository(_context);

    public ICategoryRepository Categories => 
        _categories ??= new CategoryRepository(_context);

    public ITagRepository Tags => 
        _tags ??= new TagRepository(_context);

    public IBudgetRepository Budgets => 
        _budgets ??= new BudgetRepository(_context);

    public IActivityLogRepository ActivityLogs => 
        _activityLogs ??= new ActivityLogRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        // Update timestamp for entities that have UpdatedAt property
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }

        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
