using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Utils;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gacfox.Wealthome.Services;

/// <summary>
/// 登录认证相关服务
/// </summary>
public class LoginService
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserService userService, IConfiguration configuration, AppDbContext dbContext,
        ILogger<LoginService> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public bool LoginCheck(LoginReq loginReq)
    {
        UserDto? user = _userService.GetUserByUserName(loginReq.UserName);
        // 账号不存在
        if (user == null) return false;
        // 账号被冻结
        if (user.Status == User.UserStatusSuspended) return false;
        // 密码校验不通过
        return user.PasswordHash == HashUtil.GetPasswordHash(loginReq.Password,
            _configuration.GetValue<string>("App:PasswordSha256Salt"));
    }

    public bool HasAdminUserInitialized()
    {
        return _dbContext.Users.Any();
    }

    public void InitAdminUser(InitAdminReq initAdminReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            if (HasAdminUserInitialized())
            {
                throw new BusinessException("用户表已初始化, 不能重复初始化");
            }

            if (_userService.UserExist(initAdminReq.UserName))
            {
                throw new BusinessException("相同用户名已存在");
            }

            User user = new User
            {
                UserName = initAdminReq.UserName,
                RoleCode = User.RoleCodeAdmin,
                PasswordHash = HashUtil.GetPasswordHash(initAdminReq.Password,
                    _configuration.GetValue<string>("App:PasswordSha256Salt")),
                Status = User.UserStatusActive,
                CreatedAt = DateTime.Now
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("管理员用户 [{userName}] 初始化", initAdminReq.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}