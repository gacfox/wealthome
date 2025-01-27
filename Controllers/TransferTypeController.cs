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
    public ActionResult<ApiResult<List<TransferTypeDto>>> GetTransferTypeList()
    {
        List<TransferTypeDto> transferTypeDtos = _transferTypeService.GetTransferTypeList();
        return ApiResult<List<TransferTypeDto>>.Success(transferTypeDtos);
    }

    [HttpGet("[action]")]
    public ActionResult<ApiResult<TransferTypeDto?>> GetTransferTypeById([FromQuery] long id)
    {
        TransferTypeDto? transferTypeDto = _transferTypeService.GetTransferTypeById(id);
        return ApiResult<TransferTypeDto?>.Success(transferTypeDto);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> CreateTransferType([FromBody] CreateTransferTypeReq createTransferTypeReq)
    {
        _transferTypeService.CreateTransferType(createTransferTypeReq);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> UpdateTransferType([FromBody] UpdateTransferTypeReq updateTransferTypeReq)
    {
        _transferTypeService.UpdateTransferType(updateTransferTypeReq);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> DeleteTransferType([FromBody] DeleteTransferTypeReq deleteTransferTypeReq)
    {
        _transferTypeService.DeleteTransferType(deleteTransferTypeReq.Id);
        return ApiResult<object>.Success();
    }
}