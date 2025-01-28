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
    public ActionResult<ApiResult<List<UserDto?>>> QueryUserList([FromBody] QueryUserReq queryUserReq)
    {
        return ApiResult<List<UserDto?>>.Success(_userService.GetUserList(queryUserReq.QueryDeleted));
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> CreateUser(CreateUserReq createUserReq)
    {
        _userService.CreateUser(createUserReq);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN,MEMBER")]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> UpdateUserInfo([FromBody] UpdateUserInfoReq updateUserInfoReq)
    {
        UserDto? existUser = _userService.GetUserById(updateUserInfoReq.Id);
        if (existUser == null) return ApiResult<object>.Failure("400", "用户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能更新自己的信息
            if (User.Identity?.Name != existUser.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        _userService.UpdateUser(updateUserInfoReq);

        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN,MEMBER")]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> UpdateUserAvatar([FromQuery] long id,
        [FromForm] UpdateUserAvatarReq userAvatarReq)
    {
        UserDto? existUser = _userService.GetUserById(id);
        if (existUser == null) return ApiResult<object>.Failure("400", "用户不存在");
        if (User.IsInRole(Models.User.RoleCodeMember))
        {
            // 普通用户只能更新自己的信息
            if (User.Identity?.Name != existUser.UserName) return ApiResult<object>.Failure("403", "无权限");
        }

        string prevAvatarUrl = existUser.AvatarUrl ?? string.Empty;
        string fileName = _fileService.SaveAvatar(userAvatarReq.File);
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

        _userService.UpdateUserAvatar(id, $"/api/File/DownloadFile/{fileName}");
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> EnableUser([FromBody] UpdateUserStatusReq updateUserStatusReq)
    {
        _userService.EnableUser(updateUserStatusReq.Id);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> DeleteUser([FromBody] UpdateUserStatusReq updateUserStatusReq)
    {
        _userService.DeleteUser(updateUserStatusReq.Id);
        return ApiResult<object>.Success();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> DisableUser([FromBody] UpdateUserStatusReq updateUserStatusReq)
    {
        _userService.DisableUser(updateUserStatusReq.Id);
        return ApiResult<object>.Success();
    }
}