using ExpenseManagement.Core.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace ExpenseManagement.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"], 
                _configuration["Email:FromAddress"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"], 
                int.Parse(_configuration["Email:SmtpPort"]!), 
                bool.Parse(_configuration["Email:UseSsl"]!));

            await client.AuthenticateAsync(
                _configuration["Email:Username"], 
                _configuration["Email:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken, string resetUrl)
    {
        var subject = "Reset Your Password - Expense Management";
        var body = GeneratePasswordResetEmailBody(resetToken, resetUrl);
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendBudgetAlertEmailAsync(string to, string budgetName, decimal currentAmount, decimal budgetLimit)
    {
        var subject = $"Budget Alert: {budgetName}";
        var body = GenerateBudgetAlertEmailBody(budgetName, currentAmount, budgetLimit);
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendMonthlyReportEmailAsync(string to, byte[] reportData, string fileName, DateTime reportMonth)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"], 
                _configuration["Email:FromAddress"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = $"Monthly Expense Report - {reportMonth:MMMM yyyy}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = GenerateMonthlyReportEmailBody(reportMonth);
            
            // Add attachment
            bodyBuilder.Attachments.Add(fileName, reportData, ContentType.Parse("application/pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"], 
                int.Parse(_configuration["Email:SmtpPort"]!), 
                bool.Parse(_configuration["Email:UseSsl"]!));

            await client.AuthenticateAsync(
                _configuration["Email:Username"], 
                _configuration["Email:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Monthly report email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send monthly report email to {To}", to);
            throw;
        }
    }

    private string GeneratePasswordResetEmailBody(string resetToken, string resetUrl)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #3B82F6;'>Reset Your Password</h2>
                    <p>You've requested to reset your password for your Expense Management account.</p>
                    <p>Click the button below to reset your password:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}?token={resetToken}' 
                           style='background-color: #3B82F6; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Reset Password
                        </a>
                    </div>
                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #6B7280;'>{resetUrl}?token={resetToken}</p>
                    <p><strong>This link will expire in 24 hours.</strong></p>
                    <p>If you didn't request this password reset, please ignore this email.</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #E5E7EB;'>
                    <p style='font-size: 12px; color: #9CA3AF;'>
                        This email was sent from Expense Management System.
                    </p>
                </div>
            </body>
            </html>";
    }

    private string GenerateBudgetAlertEmailBody(string budgetName, decimal currentAmount, decimal budgetLimit)
    {
        var percentage = (currentAmount / budgetLimit) * 100;
        var isExceeded = currentAmount > budgetLimit;

        return $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: {(isExceeded ? "#EF4444" : "#F59E0B")};'>
                        Budget Alert: {budgetName}
                    </h2>
                    <p>Your budget '{budgetName}' {(isExceeded ? "has been exceeded" : "is approaching its limit")}.</p>
                    <div style='background-color: #F3F4F6; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                        <p><strong>Current Spending:</strong> ${currentAmount:F2}</p>
                        <p><strong>Budget Limit:</strong> ${budgetLimit:F2}</p>
                        <p><strong>Percentage Used:</strong> {percentage:F1}%</p>
                    </div>
                    <p>Consider reviewing your expenses and adjusting your spending to stay within budget.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{_configuration["App:FrontendUrl"]}/budget' 
                           style='background-color: #3B82F6; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            View Budget Details
                        </a>
                    </div>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #E5E7EB;'>
                    <p style='font-size: 12px; color: #9CA3AF;'>
                        This email was sent from Expense Management System.
                    </p>
                </div>
            </body>
            </html>";
    }

    private string GenerateMonthlyReportEmailBody(DateTime reportMonth)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #3B82F6;'>Monthly Expense Report</h2>
                    <p>Your monthly expense report for {reportMonth:MMMM yyyy} is attached.</p>
                    <p>This report includes:</p>
                    <ul>
                        <li>Summary of all expenses for the month</li>
                        <li>Category breakdown</li>
                        <li>Budget comparison</li>
                        <li>Trends and insights</li>
                    </ul>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{_configuration["App:FrontendUrl"]}/dashboard' 
                           style='background-color: #3B82F6; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            View Dashboard
                        </a>
                    </div>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #E5E7EB;'>
                    <p style='font-size: 12px; color: #9CA3AF;'>
                        This email was sent from Expense Management System.
                    </p>
                </div>
            </body>
            </html>";
    }
}
