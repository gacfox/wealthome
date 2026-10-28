using System.Security.Claims;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly UserService _userService;

    public LoginController(LoginService loginService, UserService userService)
    {
        _loginService = loginService;
        _userService = userService;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<ApiResult<bool>>> HasAdminUserInitialized(CancellationToken cancellationToken)
    {
        bool result = await _loginService.HasAdminUserInitializedAsync(cancellationToken);
        return ApiResult<bool>.Success(result);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> InitAdminUser([FromBody] InitAdminReq initAdminReq,
        CancellationToken cancellationToken)
    {
        await _loginService.InitAdminUserAsync(initAdminReq, cancellationToken);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> Login([FromBody] LoginReq loginReq,
        CancellationToken cancellationToken)
    {
        if (!await _loginService.LoginCheckAsync(loginReq, cancellationToken))
            return ApiResult<object>.Failure("401", "用户名密码错误或账号已冻结");

        UserDto? userDto = await _userService.GetUserByUserNameAsync(loginReq.UserName, cancellationToken);
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        claimsIdentity.AddClaims(new[]
        {
            new Claim(ClaimTypes.Name, loginReq.UserName),
            new Claim(ClaimTypes.Role, userDto!.RoleCode)
        });
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        return ApiResult<object>.Success();
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<ActionResult<ApiResult<UserDto?>>> GetLoginInfo(CancellationToken cancellationToken)
    {
        UserDto? userDto = await _userService.GetUserByUserNameAsync(User.Identity!.Name!, cancellationToken);
        return ApiResult<UserDto?>.Success(userDto);
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<ActionResult<ApiResult<object>>> Logout()
    {
        await HttpContext.SignOutAsync();
        return ApiResult<object>.Success();
    }
}