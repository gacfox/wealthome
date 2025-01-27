namespace Gacfox.Wealthome.Models;

/// <summary>
/// 资金流
/// </summary>
public class Flow
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 操作账户ID
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 操作账户
    /// </summary>
    public Account Account { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 关联交易记录ID
    /// </summary>
    public long TransferId { get; set; }

    /// <summary>
    /// 关联交易记录
    /// </summary>
    public Transfer Transfer { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public class FlowDto
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public AccountDto AccountDto { get; set; }
    public decimal Amount { get; set; }
    public string AmountStr => Amount.ToString("F2");
    public long TransferId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedAtStr => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
}