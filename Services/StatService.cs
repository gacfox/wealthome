using System.Data;
using Dapper;
using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Stat;

namespace Gacfox.Wealthome.Services;

public class StatService
{
    private readonly IDbConnection _dbConnection;
    private readonly UserService _userService;

    public StatService(IDbConnection dbConnection, UserService userService)
    {
        _dbConnection = dbConnection;
        _userService = userService;
    }

    public async Task<SumStatDto> QuerySumStatAsync(QuerySumStatReq querySumStatReq, string currentLoginUserName,
        CancellationToken cancellationToken = default)
    {
        UserDto? userDto = await _userService.GetUserByUserNameAsync(currentLoginUserName, cancellationToken);
        if (userDto == null) throw new BusinessException("当前登录用户不存在");
        long currentUserId = userDto.Id;

        string sql = querySumStatReq.Mode == "total"
            ? @"
        SELECT SUM(CASE WHEN ta.account_type = 2 THEN -tf.amount ELSE 0 END) * 1.0    AS HomeIncome,
               SUM(CASE WHEN ta.account_type = 3 THEN tf.amount ELSE 0 END) * 1.0     AS HomeExpense,
               SUM(CASE WHEN tt.user_id = @CurrentUserId AND ta.account_type = 2 THEN -tf.amount ELSE 0 END) * 1.0 AS MyIncome,
               SUM(CASE WHEN tt.user_id = @CurrentUserId AND ta.account_type = 3 THEN tf.amount ELSE 0 END) * 1.0 AS MyExpense
        FROM t_transfer tt
               INNER JOIN t_flow tf ON tt.id = tf.transfer_id
               INNER JOIN t_account ta ON tf.account_id = ta.id"
            : @"
        SELECT SUM(CASE WHEN ta.account_type = 2 AND strftime('%Y', tt.created_at)= @Year THEN -tf.amount ELSE 0 END) * 1.0   AS HomeIncome,
               SUM(CASE WHEN ta.account_type = 3 AND strftime('%Y', tt.created_at)= @Year THEN tf.amount ELSE 0 END) * 1.0    AS HomeExpense,
               SUM(CASE WHEN tt.user_id = @CurrentUserId AND ta.account_type = 2 AND strftime('%Y', tt.created_at) = @Year THEN -tf.amount ELSE 0 END) * 1.0 AS MyIncome,
               SUM(CASE WHEN tt.user_id = @CurrentUserId AND ta.account_type = 3 AND strftime('%Y', tt.created_at) = @Year THEN tf.amount ELSE 0 END) * 1.0 AS MyExpense
        FROM t_transfer tt
               INNER JOIN t_flow tf ON tt.id = tf.transfer_id
               INNER JOIN t_account ta ON tf.account_id = ta.id";

        return await _dbConnection.QueryFirstAsync<SumStatDto>(new CommandDefinition(sql, new
        {
            CurrentUserId = currentUserId, querySumStatReq.Year
        }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<UserIncomeSumStatDto>> QueryUserIncomeSumStatAsync(
        QuerySumStatReq querySumStatReq, CancellationToken cancellationToken = default)
    {
        string sql = querySumStatReq.Mode == "total"
            ? @"
        SELECT tu.display_name AS DisplayName,
               SUM(CASE WHEN ta.account_type = 2 THEN -tf.amount ELSE 0 END) * 1.0 AS Income
        FROM t_transfer tt
               INNER JOIN t_flow tf ON tt.id = tf.transfer_id
               INNER JOIN t_account ta ON tf.account_id = ta.id
               INNER JOIN t_user tu ON tt.user_id = tu.id
        GROUP BY tu.id"
            : @"
        SELECT tu.display_name AS DisplayName,
               SUM(CASE WHEN ta.account_type = 2 AND strftime('%Y', tt.created_at) = @Year THEN -tf.amount ELSE 0 END) * 1.0 AS Income
        FROM t_transfer tt
               INNER JOIN t_flow tf ON tt.id = tf.transfer_id
               INNER JOIN t_account ta ON tf.account_id = ta.id
               INNER JOIN t_user tu ON tt.user_id = tu.id
        GROUP BY tu.id";
        return await _dbConnection.QueryAsync<UserIncomeSumStatDto>(new CommandDefinition(sql, new
        {
            querySumStatReq.Year
        }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<UserExpenseSumStatDto>> QueryUserExpenseSumStatAsync(
        QuerySumStatReq querySumStatReq, CancellationToken cancellationToken = default)
    {
        string sql = querySumStatReq.Mode == "total"
            ? @"
        SELECT tu.display_name AS DisplayName,
               SUM(CASE WHEN ta.account_type = 3 THEN tf.amount ELSE 0 END) * 1.0 AS Expense
        FROM t_transfer tt
               INNER JOIN t_flow tf ON tt.id = tf.transfer_id
               INNER JOIN t_account ta ON tf.account_id = ta.id
               INNER JOIN t_user tu ON tt.user_id = tu.id
        GROUP BY tu.id"
            : @"
        SELECT tu.display_name AS DisplayName,
               SUM(CASE WHEN ta.account_type = 3 AND strftime('%Y', tt.created_at) = @Year THEN tf.amount ELSE 0 END) * 1.0 AS Expense
        FROM t_transfer tt
               INNER JOIN t_flow tf ON tt.id = tf.transfer_id
               INNER JOIN t_account ta ON tf.account_id = ta.id
               INNER JOIN t_user tu ON tt.user_id = tu.id
        GROUP BY tu.id";
        return await _dbConnection.QueryAsync<UserExpenseSumStatDto>(new CommandDefinition(sql, new
        {
            querySumStatReq.Year
        }, cancellationToken: cancellationToken));
    }

    private (string, string) ExtractMonthQueryParam(QueryMonthStatReq queryMonthStatReq)
    {
        DateTime currentDate = DateTime.Now;
        DateTime start;
        DateTime end;

        if (!string.IsNullOrEmpty(queryMonthStatReq.StartMonth))
        {
            start = DateTime.ParseExact(queryMonthStatReq.StartMonth, "yyyy-MM", null);
        }
        else
        {
            start = currentDate.AddYears(-1);
            start = new DateTime(start.Year, start.Month, 1);
        }

        if (!string.IsNullOrEmpty(queryMonthStatReq.EndMonth))
        {
            end = DateTime.ParseExact(queryMonthStatReq.EndMonth, "yyyy-MM", null);
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));
            end = end.AddDays(1);
        }
        else
        {
            end = new DateTime(currentDate.Year, currentDate.Month,
                DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            end = end.AddDays(1);
        }

        string startDate = start.ToString("yyyy-MM-dd");
        string endDate = end.ToString("yyyy-MM-dd");

        return (startDate, endDate);
    }

    public async Task<IEnumerable<MonthStatDto>> QueryUserMonthStatAsync(QueryMonthStatReq queryMonthStatReq,
        string currentLoginUserName, CancellationToken cancellationToken = default)
    {
        UserDto? userDto = await _userService.GetUserByUserNameAsync(currentLoginUserName, cancellationToken);
        if (userDto == null) throw new BusinessException("当前登录用户不存在");
        long currentUserId = userDto.Id;

        (string startDate, string endDate) = ExtractMonthQueryParam(queryMonthStatReq);

        string sql = @"
        SELECT strftime('%Y-%m', tt.created_at) AS MonthStr,
               SUM(CASE WHEN ta.account_type = 2 AND tt.user_id = @CurrentUserId THEN -tf.amount ELSE 0 END) * 1.0 AS Income,
               SUM(CASE WHEN ta.account_type = 3 AND tt.user_id = @CurrentUserId THEN tf.amount ELSE 0 END) * 1.0 AS Expense
        FROM t_transfer tt
                 INNER JOIN t_flow tf ON tt.id = tf.transfer_id
                 INNER JOIN t_account ta ON tf.account_id = ta.id
        WHERE tt.created_at between @StartDate AND @EndDate
        GROUP BY MonthStr";

        return await _dbConnection.QueryAsync<MonthStatDto>(new CommandDefinition(sql, new
        {
            StartDate = startDate,
            EndDate = endDate,
            CurrentUserId = currentUserId
        }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<MonthStatDto>> QueryMonthStatAsync(QueryMonthStatReq queryMonthStatReq,
        CancellationToken cancellationToken = default)
    {
        (string startDate, string endDate) = ExtractMonthQueryParam(queryMonthStatReq);

        string sql = @"
        SELECT strftime('%Y-%m', tt.created_at) AS MonthStr,
               SUM(CASE WHEN ta.account_type = 2 THEN -tf.amount ELSE 0 END) * 1.0 AS Income,
               SUM(CASE WHEN ta.account_type = 3 THEN tf.amount ELSE 0 END) * 1.0 AS Expense
        FROM t_transfer tt
                 INNER JOIN t_flow tf ON tt.id = tf.transfer_id
                 INNER JOIN t_account ta ON tf.account_id = ta.id
        WHERE tt.created_at between @StartDate AND @EndDate
        GROUP BY MonthStr";

        return await _dbConnection.QueryAsync<MonthStatDto>(new CommandDefinition(sql, new
        {
            StartDate = startDate,
            EndDate = endDate
        }, cancellationToken: cancellationToken));
    }
}