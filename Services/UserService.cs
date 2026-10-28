using AutoMapper;
using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Utils;
using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> UserExistAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(
            o => o.UserName == userName && o.Status != User.UserStatusDeleted, cancellationToken);
    }

    public async Task<bool> UserExistIncludingDeletedAsync(string userName,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(o => o.UserName == userName, cancellationToken);
    }

    public async Task<UserDto?> GetUserByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByUserNameAsync(string userName,
        CancellationToken cancellationToken = default)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.UserName == userName, cancellationToken);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<List<UserDto?>> GetUserListAsync(bool queryDeleted,
        CancellationToken cancellationToken = default)
    {
        List<int> queryStatus = queryDeleted
            ? new List<int> { User.UserStatusDeleted }
            : new List<int> { User.UserStatusActive, User.UserStatusSuspended };
        List<User> users = await _dbContext.Users.Where(o => queryStatus.Contains(o.Status))
            .OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);
        List<UserDto?> userDtos = new List<UserDto?>();
        foreach (User user in users)
        {
            userDtos.Add(_mapper.Map<UserDto>(user));
        }

        return userDtos;
    }

    public async Task CreateUserAsync(CreateUserReq createUserReq, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? existUser = await _dbContext.Users.FirstOrDefaultAsync(o => o.UserName == createUserReq.UserName,
                cancellationToken);
            if (existUser != null) throw new BusinessException("相同用户名已存在");

            string passwordHash = HashUtil.GetPasswordHash(createUserReq.Password,
                _configuration.GetValue<string>("App:PasswordSha256Salt") ??
                    throw new InvalidOperationException("缺少配置: App:PasswordSha256Salt"));
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
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("新增用户 [{userName}]", user.UserName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task UpdateUserAsync(UpdateUserInfoReq updateUserInfoReq,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.Id == updateUserInfoReq.Id,
                cancellationToken);
            if (user == null) throw new BusinessException("用户不存在");
            user.DisplayName = updateUserInfoReq.DisplayName;
            user.Email = updateUserInfoReq.Email;
            user.RoleCode = updateUserInfoReq.RoleCode;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("用户 [{userName}] 更新信息", user.UserName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task UpdateUserAvatarAsync(long id, string avatarUrl,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (user == null) throw new BusinessException("用户不存在");
            user.AvatarUrl = avatarUrl;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("用户 [{userName}] 更新头像", user.UserName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task EnableUserAsync(long id, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (user == null) throw new BusinessException("用户不存在");
            user.Status = User.UserStatusActive;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("用户 [{userName}] 启用", user.UserName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DisableUserAsync(long id, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (user == null) throw new BusinessException("用户不存在");
            user.Status = User.UserStatusSuspended;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("用户 [{userName}] 禁用", user.UserName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DeleteUserAsync(long id, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (user == null) throw new BusinessException("用户不存在");
            user.Status = User.UserStatusDeleted;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("用户 [{userName}] 软删除", user.UserName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}