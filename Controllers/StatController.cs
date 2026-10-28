using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Models.Stat;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,MEMBER")]
public class StatController : ControllerBase
{
    private readonly StatService _statService;

    public StatController(StatService statService)
    {
        _statService = statService;
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<SumStatDto>>> QuerySumStat([FromBody] QuerySumStatReq querySumStatReq,
        CancellationToken cancellationToken)
    {
        SumStatDto sumStatDto =
            await _statService.QuerySumStatAsync(querySumStatReq, User.Identity!.Name!, cancellationToken);
        return ApiResult<SumStatDto>.Success(sumStatDto);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<IEnumerable<UserIncomeSumStatDto>>>> QueryUserIncomeSumStat(
        [FromBody] QuerySumStatReq querySumStatReq, CancellationToken cancellationToken)
    {
        IEnumerable<UserIncomeSumStatDto> userIncomeSumStatDtos =
            await _statService.QueryUserIncomeSumStatAsync(querySumStatReq, cancellationToken);
        return ApiResult<IEnumerable<UserIncomeSumStatDto>>.Success(userIncomeSumStatDtos);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<IEnumerable<UserExpenseSumStatDto>>>> QueryUserExpenseSumStat(
        [FromBody] QuerySumStatReq querySumStatReq, CancellationToken cancellationToken)
    {
        IEnumerable<UserExpenseSumStatDto> userExpenseSumStatDtos =
            await _statService.QueryUserExpenseSumStatAsync(querySumStatReq, cancellationToken);
        return ApiResult<IEnumerable<UserExpenseSumStatDto>>.Success(userExpenseSumStatDtos);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<IEnumerable<MonthStatDto>>>> QueryUserMonthStat(
        [FromBody] QueryMonthStatReq queryMonthStatReq, CancellationToken cancellationToken)
    {
        IEnumerable<MonthStatDto> userMonthStatDtos =
            await _statService.QueryUserMonthStatAsync(queryMonthStatReq, User.Identity!.Name!, cancellationToken);
        return ApiResult<IEnumerable<MonthStatDto>>.Success(userMonthStatDtos);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<IEnumerable<MonthStatDto>>>> QueryMonthStat(
        [FromBody] QueryMonthStatReq queryMonthStatReq, CancellationToken cancellationToken)
    {
        IEnumerable<MonthStatDto> userMonthStatDtos =
            await _statService.QueryMonthStatAsync(queryMonthStatReq, cancellationToken);
        return ApiResult<IEnumerable<MonthStatDto>>.Success(userMonthStatDtos);
    }
}