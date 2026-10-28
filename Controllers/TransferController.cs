using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,MEMBER")]
public class TransferController : ControllerBase
{
    private readonly TransferService _transferService;
    private readonly AccountService _accountService;

    public TransferController(TransferService transferService, AccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<Pagination<TransferDto?>>>> GetTransferListByPage(
        [FromBody] QueryTransferReq queryTransferReq, CancellationToken cancellationToken)
    {
        return ApiResult<Pagination<TransferDto?>>.Success(
            await _transferService.GetTransferListAsync(queryTransferReq, cancellationToken));
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<ApiResult<TransferDto?>>> GetTransferById([FromQuery] long id,
        CancellationToken cancellationToken)
    {
        return ApiResult<TransferDto?>.Success(
            await _transferService.GetTransferByIdAsync(id, cancellationToken));
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> CreateTransfer([FromBody] CreateTransferReq createTransferReq,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户源账户必须是自己
            AccountDto? fromAccountDto =
                await _accountService.GetAccountByIdAsync(createTransferReq.FromAccountId, cancellationToken);
            if (fromAccountDto == null) return ApiResult<object>.Failure("400", "转账来源账户不存在");
            if (User.Identity?.Name != fromAccountDto.UserDto!.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        await _transferService.CreateTransferAsync(createTransferReq, User.Identity!.Name!, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> RevertTransfer([FromBody] RevertTransferReq revertTransferReq,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能回退自己的转账记录
            TransferDto? transferDto =
                await _transferService.GetTransferByIdAsync(revertTransferReq.Id, cancellationToken);
            if (transferDto == null) return ApiResult<object>.Failure("400", "交易不存在");
            if (User.Identity?.Name != transferDto.UserDto.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        await _transferService.RevertTransferAsync(revertTransferReq, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> UpdateTransfer([FromBody] UpdateTransferReq updateTransferReq,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能更新自己的转账记录
            TransferDto? transferDto =
                await _transferService.GetTransferByIdAsync(updateTransferReq.Id, cancellationToken);
            if (transferDto == null) return ApiResult<object>.Failure("400", "交易不存在");
            if (User.Identity?.Name != transferDto.UserDto.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        await _transferService.UpdateTransferAsync(updateTransferReq, cancellationToken);
        return ApiResult<object>.Success();
    }
}