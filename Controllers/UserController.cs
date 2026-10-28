using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly FileService _fileService;

    public UserController(UserService userService, FileService fileService)
    {
        _userService = userService;
        _fileService = fileService;
    }

    [Authorize(Roles = "ADMIN,MEMBER")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<List<UserDto?>>>> QueryUserList([FromBody] QueryUserReq queryUserReq,
        CancellationToken cancellationToken)
    {
        return ApiResult<List<UserDto?>>.Success(
            await _userService.GetUserListAsync(queryUserReq.QueryDeleted, cancellationToken));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> CreateUser(CreateUserReq createUserReq,
        CancellationToken cancellationToken)
    {
        await _userService.CreateUserAsync(createUserReq, cancellationToken);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN,MEMBER")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> UpdateUserInfo([FromBody] UpdateUserInfoReq updateUserInfoReq,
        CancellationToken cancellationToken)
    {
        UserDto? existUser = await _userService.GetUserByIdAsync(updateUserInfoReq.Id, cancellationToken);
        if (existUser == null) return ApiResult<object>.Failure("400", "用户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能更新自己的信息
            if (User.Identity?.Name != existUser.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        await _userService.UpdateUserAsync(updateUserInfoReq, cancellationToken);

        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN,MEMBER")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> UpdateUserAvatar([FromQuery] long id,
        [FromForm] UpdateUserAvatarReq userAvatarReq, CancellationToken cancellationToken)
    {
        UserDto? existUser = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (existUser == null) return ApiResult<object>.Failure("400", "用户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能更新自己的信息
            if (User.Identity?.Name != existUser.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        string prevAvatarUrl = existUser.AvatarUrl ?? string.Empty;
        string fileName = await _fileService.SaveAvatarAsync(userAvatarReq.File, cancellationToken);
        if (!string.IsNullOrEmpty(prevAvatarUrl))
        {
            try
            {
                string prevFileName = prevAvatarUrl[(prevAvatarUrl.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
                _fileService.DeleteAvatar(prevFileName);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        await _userService.UpdateUserAvatarAsync(id, $"/api/File/DownloadFile/{fileName}", cancellationToken);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> EnableUser([FromBody] UpdateUserStatusReq updateUserStatusReq,
        CancellationToken cancellationToken)
    {
        await _userService.EnableUserAsync(updateUserStatusReq.Id, cancellationToken);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> DeleteUser([FromBody] UpdateUserStatusReq updateUserStatusReq,
        CancellationToken cancellationToken)
    {
        await _userService.DeleteUserAsync(updateUserStatusReq.Id, cancellationToken);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> DisableUser([FromBody] UpdateUserStatusReq updateUserStatusReq,
        CancellationToken cancellationToken)
    {
        await _userService.DisableUserAsync(updateUserStatusReq.Id, cancellationToken);
        return ApiResult<object>.Success();
    }
}