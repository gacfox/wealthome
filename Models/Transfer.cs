using System.ComponentModel.DataAnnotations;

namespace Gacfox.Wealthome.Models;

/// <summary>
/// 交易
/// </summary>
public class Transfer
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 交易类别ID
    /// </summary>
    public long TransferTypeId { get; set; }

    /// <summary>
    /// 交易类别
    /// </summary>
    public TransferType TransferType { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// 交易数据流
    /// </summary>
    public List<Flow> Flows { get; set; } = new();

    /// <summary>
    /// 交易记录创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public class CreateTransferReq
{
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(50, ErrorMessage = "标题不能超过50个字符")]
    public string Title { get; set; }

    [Required(ErrorMessage = "类别不能为空")] public long TransferTypeId { get; set; }

    [MaxLength(255, ErrorMessage = "描述不能超过255个字符")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "来源账户不能为空")] public long FromAccountId { get; set; }
    [Required(ErrorMessage = "目标账户不能为空")] public long ToAccountId { get; set; }

    [Required(ErrorMessage = "金额不能为空")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "金额必须是两位小数的定点数字符串")]
    public string AmountStr { get; set; } = "0.00";

    [Required(ErrorMessage = "时间不能为空")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$", ErrorMessage = "时间格式错误")]
    public string CreatedAtStr { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}

public class QueryTransferReq
{
    [MaxLength(50, ErrorMessage = "查询字段不能超过50个字符")]
    public string? Pattern { get; set; }

    public long? TransferUserId { get; set; }

    public long? TransferTypeId { get; set; }

    public long? IncomeAccountId { get; set; }

    public long? ExpenseAccountId { get; set; }

    public long? IncomeUserId { get; set; }

    public long? ExpenseUserId { get; set; }

    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "查询起始时间必须是yyyy-MM-dd格式")]
    public string? FromDate { get; set; }

    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "查询结束时间必须是yyyy-MM-dd格式")]
    public string? ToDate { get; set; }

    [Required(ErrorMessage = "页码不能为空")] public int PageNum { get; set; } = 1;
    [Required(ErrorMessage = "分页大小不能为空")] public int PageSize { get; set; } = 20;
}

public class RevertTransferReq
{
    [Required(ErrorMessage = "交易主键不能为空")] public long Id { get; set; }
}

public class TransferDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long TransferTypeId { get; set; }
    public TransferTypeDto TransferTypeDto { get; set; }
    public string Description { get; set; }
    public long UserId { get; set; }
    public UserDto UserDto { get; set; }
    public List<FlowDto> FlowDtos { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string CreatedAtStr => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
}

public class UpdateTransferReq
{
    [Required(ErrorMessage = "主键不能为空")] public long Id { get; set; }

    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(50, ErrorMessage = "标题不能超过50个字符")]
    public string Title { get; set; }

    [Required(ErrorMessage = "类别不能为空")] public long TransferTypeId { get; set; }

    [MaxLength(255, ErrorMessage = "描述不能超过255个字符")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "时间不能为空")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$", ErrorMessage = "时间格式错误")]
    public string CreatedAtStr { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}