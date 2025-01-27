using System.ComponentModel.DataAnnotations;

namespace Gacfox.Wealthome.Models;

/// <summary>
/// 账户
/// </summary>
public class Account
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 账户名
    /// </summary>
    public string AccountName { get; set; }

    /// <summary>
    /// 账户类型: 1 储蓄, 2 收入来源, 3 支出
    /// </summary>
    public int AccountType { get; set; }

    /// <summary>
    /// 当前余额
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 归属人ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 归属人
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 储蓄账户
    /// </summary>
    public const int AccountTypeCommon = 1;

    /// <summary>
    /// 收入来源虚拟账户
    /// </summary>
    public const int AccountTypeIncome = 2;

    /// <summary>
    /// 支出虚拟账户
    /// </summary>
    public const int AccountTypeExpense = 3;
}

public class AccountDto
{
    public long Id { get; set; }

    public string AccountName { get; set; }

    public int AccountType { get; set; }

    public string AccountTypeName
    {
        get
        {
            return AccountType switch
            {
                Account.AccountTypeCommon => "储蓄账户",
                Account.AccountTypeIncome => "收入来源虚拟账户",
                Account.AccountTypeExpense => "支出虚拟账户",
                _ => ""
            };
        }
    }

    public decimal Balance { get; set; }

    public string BalanceStr => Balance.ToString("F2");

    public long UserId { get; set; }

    public UserDto? UserDto { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedAtStr => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
}

public class CreateAccountReq
{
    [Required(ErrorMessage = "账户名不能为空")]
    [MaxLength(50, ErrorMessage = "账户名称不能超过50个字符")]
    public string AccountName { get; set; }

    [Required(ErrorMessage = "账户类型不能为空")]
    [Range(1, 3, ErrorMessage = "账户类型必须是1, 2, 3")]
    public int AccountType { get; set; }

    [Required(ErrorMessage = "账户余额不能为空")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "账户余额必须是两位小数的定点数字符串")]
    public string Balance { get; set; } = "0.00";
}

public class UpdateAccountReq
{
    [Required(ErrorMessage = "账户主键不能为空")] public long Id { get; set; }

    [Required(ErrorMessage = "账户名不能为空")]
    [MaxLength(50, ErrorMessage = "账户名称不能超过50个字符")]
    public string AccountName { get; set; }

    [Required(ErrorMessage = "账户类型不能为空")]
    [Range(1, 3, ErrorMessage = "账户类型必须是1, 2, 3")]
    public int AccountType { get; set; }
}

public class DeleteAccountReq
{
    [Required(ErrorMessage = "账户主键不能为空")] public long Id { get; set; }
}