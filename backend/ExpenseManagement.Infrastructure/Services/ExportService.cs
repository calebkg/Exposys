using System.Text;
using ClosedXML.Excel;
using ExpenseManagement.Core.Entities;
using ExpenseManagement.Core.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;

namespace ExpenseManagement.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportService> _logger;

    public ExportService(IUnitOfWork unitOfWork, ILogger<ExportService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<byte[]> ExportExpensesToExcelAsync(IEnumerable<Expense> expenses)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Expenses");

            // Headers
            var headers = new[] { "Date", "Title", "Description", "Amount", "Category", "Tags" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Data
            var row = 2;
            foreach (var expense in expenses)
            {
                worksheet.Cell(row, 1).Value = expense.Date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 2).Value = expense.Title;
                worksheet.Cell(row, 3).Value = expense.Description ?? "";
                worksheet.Cell(row, 4).Value = expense.Amount;
                worksheet.Cell(row, 5).Value = expense.Category?.Name ?? "";
                worksheet.Cell(row, 6).Value = string.Join(", ", expense.ExpenseTags?.Select(et => et.Tag.Name) ?? new List<string>());
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export expenses to Excel");
            throw;
        }
    }

    public async Task<byte[]> ExportExpensesToPdfAsync(IEnumerable<Expense> expenses)
    {
        try
        {
            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var title = new Paragraph("Expense Report", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Date range
            var dateRange = new Paragraph($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC", FontFactory.GetFont(FontFactory.HELVETICA, 10))
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 20
            };
            document.Add(dateRange);

            // Table
            var table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 15, 25, 30, 15, 15 });

            // Headers
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var headers = new[] { "Date", "Title", "Description", "Amount", "Category" };
            foreach (var header in headers)
            {
                var cell = new PdfPCell(new Phrase(header, headerFont))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            // Data
            var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            decimal totalAmount = 0;

            foreach (var expense in expenses)
            {
                table.AddCell(new PdfPCell(new Phrase(expense.Date.ToString("yyyy-MM-dd"), dataFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(expense.Title, dataFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(expense.Description ?? "", dataFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase($"${expense.Amount:F2}", dataFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(expense.Category?.Name ?? "", dataFont)) { Padding = 5 });
                totalAmount += expense.Amount;
            }

            // Total row
            var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            table.AddCell(new PdfPCell(new Phrase("", totalFont)) { Padding = 5, Border = 0 });
            table.AddCell(new PdfPCell(new Phrase("", totalFont)) { Padding = 5, Border = 0 });
            table.AddCell(new PdfPCell(new Phrase("TOTAL:", totalFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT, BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase($"${totalAmount:F2}", totalFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT, BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("", totalFont)) { Padding = 5, Border = 0 });

            document.Add(table);
            document.Close();

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export expenses to PDF");
            throw;
        }
    }

    public async Task<byte[]> GenerateMonthlyReportAsync(int userId, DateTime month)
    {
        try
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get expenses for the month
            var (expenses, _) = await _unitOfWork.Expenses.GetPagedAsync(
                1, int.MaxValue, userId, startDate, endDate);

            // Get summary data
            var totalAmount = await _unitOfWork.Expenses.GetTotalAmountAsync(userId, startDate, endDate);
            var categoryBreakdown = await _unitOfWork.Expenses.GetCategoryBreakdownAsync(userId, startDate, endDate);
            var budgets = await _unitOfWork.Budgets.GetByUserIdAsync(userId);

            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
            var title = new Paragraph($"Monthly Expense Report - {month:MMMM yyyy}", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 30
            };
            document.Add(title);

            // Summary section
            var summaryTitle = new Paragraph("Summary", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16))
            {
                SpacingAfter = 10
            };
            document.Add(summaryTitle);

            var summaryTable = new PdfPTable(2) { WidthPercentage = 50 };
            summaryTable.SetWidths(new float[] { 70, 30 });

            AddSummaryRow(summaryTable, "Total Expenses:", $"${totalAmount:F2}");
            AddSummaryRow(summaryTable, "Number of Transactions:", expenses.Count().ToString());
            AddSummaryRow(summaryTable, "Average per Transaction:", expenses.Any() ? $"${totalAmount / expenses.Count():F2}" : "$0.00");

            document.Add(summaryTable);
            document.Add(new Paragraph(" ") { SpacingAfter = 20 });

            // Category breakdown
            if (categoryBreakdown.Any())
            {
                var categoryTitle = new Paragraph("Category Breakdown", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16))
                {
                    SpacingAfter = 10
                };
                document.Add(categoryTitle);

                var categoryTable = new PdfPTable(3) { WidthPercentage = 80 };
                categoryTable.SetWidths(new float[] { 50, 25, 25 });

                // Headers
                AddHeaderCell(categoryTable, "Category");
                AddHeaderCell(categoryTable, "Amount");
                AddHeaderCell(categoryTable, "Percentage");

                foreach (var category in categoryBreakdown.OrderByDescending(c => c.Amount))
                {
                    var percentage = totalAmount > 0 ? (category.Amount / totalAmount) * 100 : 0;
                    AddDataCell(categoryTable, category.CategoryName);
                    AddDataCell(categoryTable, $"${category.Amount:F2}");
                    AddDataCell(categoryTable, $"{percentage:F1}%");
                }

                document.Add(categoryTable);
                document.Add(new Paragraph(" ") { SpacingAfter = 20 });
            }

            // Detailed transactions
            var transactionTitle = new Paragraph("Detailed Transactions", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16))
            {
                SpacingAfter = 10
            };
            document.Add(transactionTitle);

            var transactionTable = new PdfPTable(4) { WidthPercentage = 100 };
            transactionTable.SetWidths(new float[] { 20, 40, 20, 20 });

            // Headers
            AddHeaderCell(transactionTable, "Date");
            AddHeaderCell(transactionTable, "Title");
            AddHeaderCell(transactionTable, "Category");
            AddHeaderCell(transactionTable, "Amount");

            foreach (var expense in expenses.OrderByDescending(e => e.Date))
            {
                AddDataCell(transactionTable, expense.Date.ToString("yyyy-MM-dd"));
                AddDataCell(transactionTable, expense.Title);
                AddDataCell(transactionTable, expense.Category?.Name ?? "");
                AddDataCell(transactionTable, $"${expense.Amount:F2}");
            }

            document.Add(transactionTable);
            document.Close();

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate monthly report for user {UserId}", userId);
            throw;
        }
    }

    private void AddSummaryRow(PdfPTable table, string label, string value)
    {
        var labelCell = new PdfPCell(new Phrase(label, FontFactory.GetFont(FontFactory.HELVETICA, 12))) { Padding = 5, Border = 0 };
        var valueCell = new PdfPCell(new Phrase(value, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12))) { Padding = 5, Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
        table.AddCell(labelCell);
        table.AddCell(valueCell);
    }

    private void AddHeaderCell(PdfPTable table, string text)
    {
        var cell = new PdfPCell(new Phrase(text, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
        {
            BackgroundColor = BaseColor.LIGHT_GRAY,
            HorizontalAlignment = Element.ALIGN_CENTER,
            Padding = 5
        };
        table.AddCell(cell);
    }

    private void AddDataCell(PdfPTable table, string text)
    {
        var cell = new PdfPCell(new Phrase(text, FontFactory.GetFont(FontFactory.HELVETICA, 10))) { Padding = 5 };
        table.AddCell(cell);
    }
}
