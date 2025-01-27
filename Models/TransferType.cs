using System.ComponentModel.DataAnnotations;

namespace Gacfox.Wealthome.Models;

/// <summary>
/// 交易类别
/// </summary>
public class TransferType
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 类别名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 收入支出类型: 1 收入类型, 2 支出类型, 3 资金在个人账号内流动类型
    /// </summary>
    public int InoutType { get; set; }

    /// <summary>
    /// 收入类型
    /// </summary>
    public const int TransferTypeIncome = 1;

    /// <summary>
    /// 支出类型
    /// </summary>
    public const int TransferTypeExpense = 2;

    /// <summary>
    /// 内部转账类型
    /// </summary>
    public const int TransferTypeInternal = 3;
}

public class TransferTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int InoutType { get; set; }

    public string InoutTypeName
    {
        get
        {
            return InoutType switch
            {
                TransferType.TransferTypeIncome => "收入",
                TransferType.TransferTypeExpense => "支出",
                TransferType.TransferTypeInternal => "内部转账",
                _ => ""
            };
        }
    }
}

public class CreateTransferTypeReq
{
    [Required(ErrorMessage = "交易类别名不能为空")]
    [MaxLength(20, ErrorMessage = "交易类别名长度不能超过20个字符")]
    public string Name { get; set; }

    [Required(ErrorMessage = "收入支出类型不能为空")]
    [Range(1, 3, ErrorMessage = "收入支出类型值必须是1, 2, 3")]
    public int InoutType { get; set; }
}

public class UpdateTransferTypeReq
{
    [Required(ErrorMessage = "交易类别主键不能为空")]
    public long Id { get; set; }

    [Required(ErrorMessage = "交易类别名不能为空")]
    [MaxLength(20, ErrorMessage = "交易类别名长度不能超过20个字符")]
    public string Name { get; set; }

    [Required(ErrorMessage = "收入支出类型不能为空")]
    [Range(1, 3, ErrorMessage = "收入支出类型值必须是1, 2, 3")]
    public int InoutType { get; set; }
}

public class DeleteTransferTypeReq
{
    [Required(ErrorMessage = "类型主键不能为空")] public long Id { get; set; }
}