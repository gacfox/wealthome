using System.ComponentModel.DataAnnotations;

namespace Gacfox.Wealthome.Models.Stat;

public class MonthStatDto
{
    public string MonthStr { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public string IncomeStr => Income.ToString("F2");
    public string ExpenseStr => Expense.ToString("F2");
}

public class QueryMonthStatReq
{
    [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "月份格式错误")]
    public string? StartMonth { get; set; }

    [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "月份格式错误")]
    public string? EndMonth { get; set; }
}