using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,MEMBER")]
public class TransferTypeController : ControllerBase
{
    private readonly TransferTypeService _transferTypeService;

    public TransferTypeController(TransferTypeService transferTypeService)
    {
        _transferTypeService = transferTypeService;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<ApiResult<List<TransferTypeDto>>>> GetTransferTypeList(
        CancellationToken cancellationToken)
    {
        List<TransferTypeDto> transferTypeDtos =
            await _transferTypeService.GetTransferTypeListAsync(cancellationToken);
        return ApiResult<List<TransferTypeDto>>.Success(transferTypeDtos);
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<ApiResult<TransferTypeDto?>>> GetTransferTypeById([FromQuery] long id,
        CancellationToken cancellationToken)
    {
        TransferTypeDto? transferTypeDto =
            await _transferTypeService.GetTransferTypeByIdAsync(id, cancellationToken);
        return ApiResult<TransferTypeDto?>.Success(transferTypeDto);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> CreateTransferType(
        [FromBody] CreateTransferTypeReq createTransferTypeReq, CancellationToken cancellationToken)
    {
        await _transferTypeService.CreateTransferTypeAsync(createTransferTypeReq, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> UpdateTransferType(
        [FromBody] UpdateTransferTypeReq updateTransferTypeReq, CancellationToken cancellationToken)
    {
        await _transferTypeService.UpdateTransferTypeAsync(updateTransferTypeReq, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> DeleteTransferType(
        [FromBody] DeleteTransferTypeReq deleteTransferTypeReq, CancellationToken cancellationToken)
    {
        await _transferTypeService.DeleteTransferTypeAsync(deleteTransferTypeReq.Id, cancellationToken);
        return ApiResult<object>.Success();
    }
}