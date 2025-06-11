using ExpenseManagement.Core.Entities;
using ExpenseManagement.Core.Interfaces;
using ExpenseManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Infrastructure.Repositories;

public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<ActivityLog> logs, int totalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        int? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        string? action = null, 
        string? entity = null)
    {
        var query = _dbSet.Include(al => al.User).AsQueryable();

        if (userId.HasValue)
            query = query.Where(al => al.UserId == userId.Value);

        if (startDate.HasValue)
            query = query.Where(al => al.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(al => al.Timestamp <= endDate.Value);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(al => al.Action == action);

        if (!string.IsNullOrWhiteSpace(entity))
            query = query.Where(al => al.Entity == entity);

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(al => al.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (logs, totalCount);
    }

    public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId, int take = 50)
    {
        return await _dbSet
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.Timestamp)
            .Take(take)
            .ToListAsync();
    }
}
