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
    public ActionResult<ApiResult<bool>> HasAdminUserInitialized()
    {
        bool result = _loginService.HasAdminUserInitialized();
        return ApiResult<bool>.Success(result);
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> InitAdminUser([FromBody] InitAdminReq initAdminReq)
    {
        _loginService.InitAdminUser(initAdminReq);
        return ApiResult<object>.Success();
    }

    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> Login([FromBody] LoginReq loginReq)
    {
        if (!_loginService.LoginCheck(loginReq)) return ApiResult<object>.Failure("401", "用户名密码错误或账号已冻结");

        UserDto? userDto = _userService.GetUserByUserName(loginReq.UserName);
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        claimsIdentity.AddClaims(new[]
        {
            new Claim(ClaimTypes.Name, loginReq.UserName),
            new Claim(ClaimTypes.Role, userDto!.RoleCode)
        });
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        return ApiResult<object>.Success();
    }

    [Authorize]
    [HttpGet("[action]")]
    public ActionResult<ApiResult<UserDto?>> GetLoginInfo()
    {
        UserDto? userDto = _userService.GetUserByUserName(User.Identity!.Name!);
        return ApiResult<UserDto?>.Success(userDto);
    }

    [Authorize]
    [HttpPost("[action]")]
    public ActionResult<ApiResult<object>> Logout()
    {
        HttpContext.SignOutAsync();
        return ApiResult<object>.Success();
    }
}