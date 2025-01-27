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
    public ActionResult<ApiResult<SumStatDto>> QuerySumStat([FromBody] QuerySumStatReq querySumStatReq)
    {
        SumStatDto sumStatDto = _statService.QuerySumStat(querySumStatReq, User.Identity!.Name!);
        return ApiResult<SumStatDto>.Success(sumStatDto);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<IEnumerable<UserIncomeSumStatDto>>> QueryUserIncomeSumStat(
        [FromBody] QuerySumStatReq querySumStatReq)
    {
        IEnumerable<UserIncomeSumStatDto> userIncomeSumStatDtos = _statService.QueryUserIncomeSumStat(querySumStatReq);
        return ApiResult<IEnumerable<UserIncomeSumStatDto>>.Success(userIncomeSumStatDtos);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<IEnumerable<UserExpenseSumStatDto>>> QueryUserExpenseSumStat(
        [FromBody] QuerySumStatReq querySumStatReq)
    {
        IEnumerable<UserExpenseSumStatDto> userExpenseSumStatDtos =
            _statService.QueryUserExpenseSumStat(querySumStatReq);
        return ApiResult<IEnumerable<UserExpenseSumStatDto>>.Success(userExpenseSumStatDtos);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<IEnumerable<MonthStatDto>>> QueryUserMonthStat(
        [FromBody] QueryMonthStatReq queryMonthStatReq)
    {
        IEnumerable<MonthStatDto> userMonthStatDtos =
            _statService.QueryUserMonthStat(queryMonthStatReq, User.Identity!.Name!);
        return ApiResult<IEnumerable<MonthStatDto>>.Success(userMonthStatDtos);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<IEnumerable<MonthStatDto>>> QueryMonthStat(
        [FromBody] QueryMonthStatReq queryMonthStatReq)
    {
        IEnumerable<MonthStatDto> userMonthStatDtos =
            _statService.QueryMonthStat(queryMonthStatReq);
        return ApiResult<IEnumerable<MonthStatDto>>.Success(userMonthStatDtos);
    }
}