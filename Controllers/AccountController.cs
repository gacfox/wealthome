using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,MEMBER")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("[action]")]
    public ActionResult<ApiResult<List<AccountDto?>>> GetAccountList()
    {
        List<AccountDto?> accountDtos = _accountService.GetAccountList();
        return ApiResult<List<AccountDto?>>.Success(accountDtos);
    }

    [HttpGet("[action]")]
    public ActionResult<ApiResult<AccountDto?>> GetAccountById([FromQuery] long id)
    {
        AccountDto? accountDto = _accountService.GetAccountById(id);
        return ApiResult<AccountDto?>.Success(accountDto);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> CreateAccount([FromBody] CreateAccountReq createAccountReq)
    {
        string currentLoginUsername = User.Identity!.Name!;
        _accountService.CreateAccount(createAccountReq, currentLoginUsername);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> UpdateAccount([FromBody] UpdateAccountReq updateAccountReq)
    {
        AccountDto? accountDto = _accountService.GetAccountById(updateAccountReq.Id);
        if (accountDto == null) return ApiResult<object>.Failure("400", "账户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember) && User.Identity?.Name != accountDto.UserDto!.UserName)
            return ApiResult<object>.Failure("403", "无权限");
        _accountService.UpdateAccount(updateAccountReq);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> DeleteAccount([FromBody] DeleteAccountReq deleteAccountReq)
    {
        AccountDto? accountDto = _accountService.GetAccountById(deleteAccountReq.Id);
        if (accountDto == null) return ApiResult<object>.Failure("400", "账户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember) && User.Identity?.Name != accountDto.UserDto!.UserName)
            return ApiResult<object>.Failure("403", "无权限");
        _accountService.DeleteAccount(deleteAccountReq.Id);
        return ApiResult<object>.Success();
    }
}