namespace ExpenseManagement.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendPasswordResetEmailAsync(string to, string resetToken, string resetUrl);
    Task SendBudgetAlertEmailAsync(string to, string budgetName, decimal currentAmount, decimal budgetLimit);
    Task SendMonthlyReportEmailAsync(string to, byte[] reportData, string fileName, DateTime reportMonth);
}

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
}

public interface IExportService
{
    Task<byte[]> ExportExpensesToExcelAsync(IEnumerable<Entities.Expense> expenses);
    Task<byte[]> ExportExpensesToPdfAsync(IEnumerable<Entities.Expense> expenses);
    Task<byte[]> GenerateMonthlyReportAsync(int userId, DateTime month);
}

public interface INotificationService
{
    Task SendBudgetAlertAsync(int budgetId, AlertType alertType);
    Task SendMonthlyReportAsync(int userId, DateTime month);
    Task<IEnumerable<Entities.BudgetAlert>> GetUnreadAlertsAsync(int userId);
    Task MarkAlertAsReadAsync(int alertId);
}

public interface IActivityLogService
{
    Task LogActivityAsync(string action, string entity, int entityId, int userId, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null);
    Task<(IEnumerable<Entities.ActivityLog> logs, int totalCount)> GetActivityLogsAsync(int page, int pageSize, int? userId = null, DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entity = null);
    Task<IEnumerable<Entities.ActivityLog>> GetUserActivityLogsAsync(int userId, int take = 50);
}

using ExpenseManagement.Core.Entities;
