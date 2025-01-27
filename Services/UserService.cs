using AutoMapper;
using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Utils;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gacfox.Wealthome.Services;

public class UserService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private ILogger<LoginService> _logger;

    public UserService(IConfiguration configuration, AppDbContext dbContext, IMapper mapper,
        ILogger<LoginService> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public bool UserExist(string userName)
    {
        return _dbContext.Users.Any(o => o.UserName == userName && o.Status != User.UserStatusDeleted);
    }

    public bool UserExistIncludingDeleted(string userName)
    {
        return _dbContext.Users.Any(o => o.UserName == userName);
    }

    public UserDto? GetUserById(long id)
    {
        User? user = _dbContext.Users.FirstOrDefault(o => o.Id == id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public UserDto? GetUserByUserName(string userName)
    {
        User? user = _dbContext.Users.FirstOrDefault(o => o.UserName == userName);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public List<UserDto?> GetUserList(bool queryDeleted)
    {
        List<int> queryStatus = queryDeleted
            ? new List<int> { User.UserStatusDeleted }
            : new List<int> { User.UserStatusActive, User.UserStatusSuspended };
        List<User> users = _dbContext.Users.Where(o => queryStatus.Contains(o.Status))
            .OrderByDescending(o => o.CreatedAt).ToList();
        List<UserDto?> userDtos = new List<UserDto?>();
        foreach (User user in users)
        {
            userDtos.Add(_mapper.Map<UserDto>(user));
        }

        return userDtos;
    }

    public void CreateUser(CreateUserReq createUserReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? existUser = _dbContext.Users.FirstOrDefault(o => o.UserName == createUserReq.UserName);
            if (existUser != null) throw new BusinessException("相同用户名已存在");

            string passwordHash = HashUtil.GetPasswordHash(createUserReq.Password,
                _configuration.GetValue<string>("App:PasswordSha256Salt"));
            User user = new User
            {
                UserName = createUserReq.UserName,
                DisplayName = createUserReq.DisplayName,
                Email = createUserReq.Email,
                AvatarUrl = createUserReq.AvatarUrl,
                RoleCode = createUserReq.RoleCode,
                PasswordHash = passwordHash,
                Status = User.UserStatusActive,
                CreatedAt = DateTime.Now
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("新增用户 [{userName}]", user.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateUser(UpdateUserInfoReq updateUserInfoReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? user = _dbContext.Users.FirstOrDefault(o => o.Id == updateUserInfoReq.Id);
            if (user == null) throw new BusinessException("用户不存在");
            user.DisplayName = updateUserInfoReq.DisplayName;
            user.Email = updateUserInfoReq.Email;
            user.RoleCode = updateUserInfoReq.RoleCode;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("用户 [{userName}] 更新信息", user.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateUserAvatar(long id, string avatarUrl)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? user = _dbContext.Users.FirstOrDefault(o => o.Id == id);
            if (user == null) throw new BusinessException("用户不存在");
            user.AvatarUrl = avatarUrl;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("用户 [{userName}] 更新头像", user.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void EnableUser(long id)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? user = _dbContext.Users.FirstOrDefault(o => o.Id == id);
            if (user == null) throw new BusinessException("用户不存在");
            user.Status = User.UserStatusActive;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("用户 [{userName}] 启用", user.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void DisableUser(long id)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? user = _dbContext.Users.FirstOrDefault(o => o.Id == id);
            if (user == null) throw new BusinessException("用户不存在");
            user.Status = User.UserStatusSuspended;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("用户 [{userName}] 禁用", user.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void DeleteUser(long id)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? user = _dbContext.Users.FirstOrDefault(o => o.Id == id);
            if (user == null) throw new BusinessException("用户不存在");
            user.Status = User.UserStatusDeleted;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("用户 [{userName}] 软删除", user.UserName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}