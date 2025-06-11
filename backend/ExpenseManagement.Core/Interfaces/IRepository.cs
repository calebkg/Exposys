using System.Linq.Expressions;

namespace ExpenseManagement.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}

public interface IExpenseRepository : IRepository<Entities.Expense>
{
    Task<(IEnumerable<Entities.Expense> expenses, int totalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        int userId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int[]? categoryIds = null, 
        int[]? tagIds = null, 
        decimal? minAmount = null, 
        decimal? maxAmount = null, 
        string? searchTerm = null);
    
    Task<decimal> GetTotalAmountAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<(int CategoryId, string CategoryName, decimal Amount, int Count)>> GetCategoryBreakdownAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<(string Month, int Year, decimal Amount, int Count)>> GetMonthlyTrendAsync(int userId, int months = 12);
}

public interface ICategoryRepository : IRepository<Entities.Category>
{
    Task<IEnumerable<Entities.Category>> GetByUserIdAsync(int userId);
}

public interface ITagRepository : IRepository<Entities.Tag>
{
    Task<IEnumerable<Entities.Tag>> GetByUserIdAsync(int userId);
}

public interface IBudgetRepository : IRepository<Entities.Budget>
{
    Task<IEnumerable<Entities.Budget>> GetByUserIdAsync(int userId);
    Task<decimal> GetCurrentSpentAsync(int budgetId);
    Task<IEnumerable<Entities.Budget>> GetExceededBudgetsAsync(int userId);
}

public interface IActivityLogRepository : IRepository<Entities.ActivityLog>
{
    Task<(IEnumerable<Entities.ActivityLog> logs, int totalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        int? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        string? action = null, 
        string? entity = null);
    
    Task<IEnumerable<Entities.ActivityLog>> GetByUserIdAsync(int userId, int take = 50);
}

public interface IUnitOfWork : IDisposable
{
    IExpenseRepository Expenses { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IBudgetRepository Budgets { get; }
    IActivityLogRepository ActivityLogs { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
