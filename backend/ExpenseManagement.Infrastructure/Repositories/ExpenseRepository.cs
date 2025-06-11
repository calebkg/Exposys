using ExpenseManagement.Core.Entities;
using ExpenseManagement.Core.Interfaces;
using ExpenseManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Infrastructure.Repositories;

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Expense> expenses, int totalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        int userId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int[]? categoryIds = null, 
        int[]? tagIds = null, 
        decimal? minAmount = null, 
        decimal? maxAmount = null, 
        string? searchTerm = null)
    {
        var query = _dbSet
            .Include(e => e.Category)
            .Include(e => e.ExpenseTags)
                .ThenInclude(et => et.Tag)
            .Where(e => e.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);

        if (categoryIds != null && categoryIds.Length > 0)
            query = query.Where(e => categoryIds.Contains(e.CategoryId));

        if (tagIds != null && tagIds.Length > 0)
            query = query.Where(e => e.ExpenseTags.Any(et => tagIds.Contains(et.TagId)));

        if (minAmount.HasValue)
            query = query.Where(e => e.Amount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(e => e.Amount <= maxAmount.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e => 
                e.Title.Contains(searchTerm) || 
                (e.Description != null && e.Description.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (expenses, totalCount);
    }

    public async Task<decimal> GetTotalAmountAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(e => e.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);

        return await query.SumAsync(e => e.Amount);
    }

    public async Task<IEnumerable<(int CategoryId, string CategoryName, decimal Amount, int Count)>> GetCategoryBreakdownAsync(
        int userId, 
        DateTime? startDate = null, 
        DateTime? endDate = null)
    {
        var query = _dbSet
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);

        return await query
            .GroupBy(e => new { e.CategoryId, e.Category.Name })
            .Select(g => new 
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Amount = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .Select(x => ValueTuple.Create(x.CategoryId, x.CategoryName, x.Amount, x.Count))
            .ToListAsync();
    }

    public async Task<IEnumerable<(string Month, int Year, decimal Amount, int Count)>> GetMonthlyTrendAsync(int userId, int months = 12)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months);

        return await _dbSet
            .Where(e => e.UserId == userId && e.Date >= startDate)
            .GroupBy(e => new { Year = e.Date.Year, Month = e.Date.Month })
            .Select(g => new 
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Amount = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .Select(x => ValueTuple.Create(
                System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month), 
                x.Year, 
                x.Amount, 
                x.Count))
            .ToListAsync();
    }

    public override async Task<Expense?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Category)
            .Include(e => e.ExpenseTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
