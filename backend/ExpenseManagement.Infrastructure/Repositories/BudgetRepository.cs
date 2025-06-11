using ExpenseManagement.Core.Entities;
using ExpenseManagement.Core.Interfaces;
using ExpenseManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Infrastructure.Repositories;

public class BudgetRepository : Repository<Budget>, IBudgetRepository
{
    public BudgetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Budget>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(b => b.Category)
            .Include(b => b.BudgetAlerts)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetCurrentSpentAsync(int budgetId)
    {
        var budget = await _dbSet
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == budgetId);

        if (budget == null) return 0;

        var query = _context.Expenses
            .Where(e => e.UserId == budget.UserId)
            .Where(e => e.Date >= budget.StartDate && e.Date <= budget.EndDate);

        // If budget is category-specific, filter by category
        if (budget.CategoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == budget.CategoryId.Value);
        }

        return await query.SumAsync(e => e.Amount);
    }

    public async Task<IEnumerable<Budget>> GetExceededBudgetsAsync(int userId)
    {
        var budgets = await GetByUserIdAsync(userId);
        var exceededBudgets = new List<Budget>();

        foreach (var budget in budgets)
        {
            var currentSpent = await GetCurrentSpentAsync(budget.Id);
            var threshold = budget.Amount * (budget.AlertThreshold / 100);

            if (currentSpent >= threshold)
            {
                exceededBudgets.Add(budget);
            }
        }

        return exceededBudgets;
    }

    public override async Task<Budget?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Category)
            .Include(b => b.BudgetAlerts)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}
