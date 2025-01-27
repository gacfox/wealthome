namespace Gacfox.Wealthome.Exceptions;

/// <summary>
/// 业务逻辑异常
/// </summary>
public class BusinessException : Exception
{
    public BusinessException()
    {
    }

    public BusinessException(string message) : base(message)
    {
    }
}