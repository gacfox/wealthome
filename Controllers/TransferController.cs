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
    public ActionResult<ApiResult<Pagination<TransferDto?>>> GetTransferListByPage(
        [FromBody] QueryTransferReq queryTransferReq)
    {
        return ApiResult<Pagination<TransferDto?>>.Success(_transferService.GetTransferList(queryTransferReq));
    }

    [HttpGet("[action]")]
    public ActionResult<ApiResult<TransferDto?>> GetTransferById([FromQuery] long id)
    {
        return ApiResult<TransferDto?>.Success(_transferService.GetTransferById(id));
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> CreateTransfer([FromBody] CreateTransferReq createTransferReq)
    {
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户源账户必须是自己
            AccountDto? fromAccountDto = _accountService.GetAccountById(createTransferReq.FromAccountId);
            if (fromAccountDto == null) return ApiResult<object>.Failure("400", "转账来源账户不存在");
            if (User.Identity?.Name != fromAccountDto.UserDto!.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        _transferService.CreateTransfer(createTransferReq, User.Identity!.Name!);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> RevertTransfer([FromBody] RevertTransferReq revertTransferReq)
    {
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能回退自己的转账记录
            TransferDto? transferDto = _transferService.GetTransferById(revertTransferReq.Id);
            if (transferDto == null) return ApiResult<object>.Failure("400", "交易不存在");
            if (User.Identity?.Name != transferDto.UserDto.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        _transferService.RevertTransfer(revertTransferReq);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> UpdateTransfer([FromBody] UpdateTransferReq updateTransferReq)
    {
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能更新自己的转账记录
            TransferDto? transferDto = _transferService.GetTransferById(updateTransferReq.Id);
            if (transferDto == null) return ApiResult<object>.Failure("400", "交易不存在");
            if (User.Identity?.Name != transferDto.UserDto.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        _transferService.UpdateTransfer(updateTransferReq);
        return ApiResult<object>.Success();
    }
}