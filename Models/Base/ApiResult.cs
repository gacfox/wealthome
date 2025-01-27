namespace Gacfox.Wealthome.Models.Base;

/// <summary>
/// 响应体封装
/// </summary>
/// <typeparam name="T">响应数据泛型类型</typeparam>
public class ApiResult<T>
{
    public string Code { get; set; }

    public string Message { get; set; }

    public T? Data { get; set; }

    public ApiResult()
    {
        Code = "0";
        Message = "success";
    }

    public ApiResult(string code, string message, T? data = default)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public static ApiResult<object> Success()
    {
        return new ApiResult<object>("0", "success");
    }

    public static ApiResult<T> Success(T data)
    {
        return new ApiResult<T>("0", "success", data);
    }

    public static ApiResult<object> Success(string message)
    {
        return new ApiResult<object>("0", message);
    }

    public static ApiResult<T> Success(string code, string message, T data)
    {
        return new ApiResult<T>(code, message, data);
    }

    public static ApiResult<T> Success(string message, T data)
    {
        return new ApiResult<T>("0", message, data);
    }

    public static ApiResult<object> Failure()
    {
        return new ApiResult<object>("1", "failure");
    }

    public static ApiResult<object> Failure(string message)
    {
        return new ApiResult<object>("1", message);
    }

    public static ApiResult<object> Failure(string code, string message)
    {
        return new ApiResult<object>(code, message);
    }

    public static ApiResult<object> Failure(string code, string message, string trace)
    {
        return new ApiResult<object>(code, message, trace);
    }
}