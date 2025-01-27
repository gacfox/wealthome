using System.ComponentModel.DataAnnotations;

namespace Gacfox.Wealthome.Models.Stat;

public class SumStatDto
{
    public decimal MyIncome { get; set; }
    public decimal MyExpense { get; set; }
    public decimal HomeIncome { get; set; }
    public decimal HomeExpense { get; set; }
    public string MyIncomeStr => MyIncome.ToString("F2");
    public string MyExpenseStr => MyExpense.ToString("F2");
    public string HomeIncomeStr => HomeIncome.ToString("F2");
    public string HomeExpenseStr => HomeExpense.ToString("F2");
}

public class UserIncomeSumStatDto
{
    public string DisplayName { get; set; }
    public decimal Income { get; set; }
    public string IncomeStr => Income.ToString("F2");
}

public class UserExpenseSumStatDto
{
    public string DisplayName { get; set; }
    public decimal Expense { get; set; }
    public string ExpenseStr => Expense.ToString("F2");
}

public class QuerySumStatReq
{
    /// <summary>
    /// 统计模式: total 全部, year 按年
    /// </summary>
    public string Mode { get; set; } = "total";

    /// <summary>
    /// 统计年份, 仅按年统计有效
    /// </summary>
    [RegularExpression(@"^\d{4}$", ErrorMessage = "年份格式错误")]
    public string? Year { get; set; }
}