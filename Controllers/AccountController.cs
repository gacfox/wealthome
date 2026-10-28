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
    public async Task<ActionResult<ApiResult<List<AccountDto?>>>> GetAccountList(
        CancellationToken cancellationToken)
    {
        List<AccountDto?> accountDtos = await _accountService.GetAccountListAsync(cancellationToken);
        return ApiResult<List<AccountDto?>>.Success(accountDtos);
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<ApiResult<AccountDto?>>> GetAccountById([FromQuery] long id,
        CancellationToken cancellationToken)
    {
        AccountDto? accountDto = await _accountService.GetAccountByIdAsync(id, cancellationToken);
        return ApiResult<AccountDto?>.Success(accountDto);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> CreateAccount([FromBody] CreateAccountReq createAccountReq,
        CancellationToken cancellationToken)
    {
        string currentLoginUsername = User.Identity!.Name!;
        await _accountService.CreateAccountAsync(createAccountReq, currentLoginUsername, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> UpdateAccount([FromBody] UpdateAccountReq updateAccountReq,
        CancellationToken cancellationToken)
    {
        AccountDto? accountDto = await _accountService.GetAccountByIdAsync(updateAccountReq.Id, cancellationToken);
        if (accountDto == null) return ApiResult<object>.Failure("400", "账户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember) && User.Identity?.Name != accountDto.UserDto!.UserName)
            return ApiResult<object>.Failure("403", "无权限");
        await _accountService.UpdateAccountAsync(updateAccountReq, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> DeleteAccount([FromBody] DeleteAccountReq deleteAccountReq,
        CancellationToken cancellationToken)
    {
        AccountDto? accountDto = await _accountService.GetAccountByIdAsync(deleteAccountReq.Id, cancellationToken);
        if (accountDto == null) return ApiResult<object>.Failure("400", "账户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember) && User.Identity?.Name != accountDto.UserDto!.UserName)
            return ApiResult<object>.Failure("403", "无权限");
        await _accountService.DeleteAccountAsync(deleteAccountReq.Id, cancellationToken);
        return ApiResult<object>.Success();
    }
}