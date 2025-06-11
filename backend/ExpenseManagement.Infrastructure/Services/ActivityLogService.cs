using System.Text.Json;
using ExpenseManagement.Core.Entities;
using ExpenseManagement.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ExpenseManagement.Infrastructure.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(IUnitOfWork unitOfWork, ILogger<ActivityLogService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task LogActivityAsync(
        string action, 
        string entity, 
        int entityId, 
        int userId, 
        string? oldValues = null, 
        string? newValues = null, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        try
        {
            var activityLog = new ActivityLog
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                UserId = userId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ipAddress ?? "",
                UserAgent = userAgent ?? "",
                Timestamp = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(activityLog);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Activity logged: {Action} {Entity} {EntityId} by User {UserId}", 
                action, entity, entityId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to log activity: {Action} {Entity} {EntityId} by User {UserId}", 
                action, entity, entityId, userId);
            // Don't throw - activity logging should not break the main flow
        }
    }

    public async Task<(IEnumerable<ActivityLog> logs, int totalCount)> GetActivityLogsAsync(
        int page, 
        int pageSize, 
        int? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        string? action = null, 
        string? entity = null)
    {
        return await _unitOfWork.ActivityLogs.GetPagedAsync(
            page, pageSize, userId, startDate, endDate, action, entity);
    }

    public async Task<IEnumerable<ActivityLog>> GetUserActivityLogsAsync(int userId, int take = 50)
    {
        return await _unitOfWork.ActivityLogs.GetByUserIdAsync(userId, take);
    }

    public string SerializeObject(object obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return obj.ToString() ?? "";
        }
    }

    public async Task LogCreateAsync<T>(T entity, int userId, string? ipAddress = null, string? userAgent = null) where T : class
    {
        var entityName = typeof(T).Name;
        var entityId = GetEntityId(entity);
        var newValues = SerializeObject(entity);

        await LogActivityAsync(
            ActivityAction.Create.ToString(), 
            entityName, 
            entityId, 
            userId, 
            null, 
            newValues, 
            ipAddress, 
            userAgent);
    }

    public async Task LogUpdateAsync<T>(T oldEntity, T newEntity, int userId, string? ipAddress = null, string? userAgent = null) where T : class
    {
        var entityName = typeof(T).Name;
        var entityId = GetEntityId(newEntity);
        var oldValues = SerializeObject(oldEntity);
        var newValues = SerializeObject(newEntity);

        await LogActivityAsync(
            ActivityAction.Update.ToString(), 
            entityName, 
            entityId, 
            userId, 
            oldValues, 
            newValues, 
            ipAddress, 
            userAgent);
    }

    public async Task LogDeleteAsync<T>(T entity, int userId, string? ipAddress = null, string? userAgent = null) where T : class
    {
        var entityName = typeof(T).Name;
        var entityId = GetEntityId(entity);
        var oldValues = SerializeObject(entity);

        await LogActivityAsync(
            ActivityAction.Delete.ToString(), 
            entityName, 
            entityId, 
            userId, 
            oldValues, 
            null, 
            ipAddress, 
            userAgent);
    }

    private int GetEntityId(object entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(int))
        {
            return (int)idProperty.GetValue(entity)!;
        }
        return 0;
    }
}
