using AutoMapper;
using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gacfox.Wealthome.Services;

public class AccountService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginService> _logger;

    public AccountService(AppDbContext dbContext, IMapper mapper, ILogger<LoginService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<AccountDto?>> GetAccountListAsync(CancellationToken cancellationToken = default)
    {
        List<AccountDto?> accountDtos = new List<AccountDto?>();
        List<Account> accounts = await _dbContext.Accounts.Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);
        foreach (Account account in accounts)
        {
            AccountDto? accountDto = _mapper.Map<AccountDto>(account);
            if (accountDto == null) continue;
            User u = account.User;
            UserDto? userDto = _mapper.Map<UserDto>(u);
            accountDto.UserDto = userDto;
            accountDtos.Add(accountDto);
        }

        return accountDtos;
    }

    public async Task<AccountDto?> GetAccountByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        Account? account = await _dbContext.Accounts.Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        AccountDto? accountDto = _mapper.Map<AccountDto>(account);
        if (account == null || accountDto == null) return null;
        User u = account.User;
        UserDto? userDto = _mapper.Map<UserDto>(u);
        accountDto.UserDto = userDto;
        return accountDto;
    }

    public async Task CreateAccountAsync(CreateAccountReq createAccountReq, string currentLoginUserName,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(o => o.UserName == currentLoginUserName,
                cancellationToken);
            if (user == null) throw new BusinessException("当前登录用户不存在");

            Account account = new Account
            {
                AccountName = createAccountReq.AccountName,
                AccountType = createAccountReq.AccountType,
                Balance = decimal.Parse(createAccountReq.Balance),
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.Now,
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("新增账户 [{accountName}]", account.AccountName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task UpdateAccountAsync(UpdateAccountReq updateAccountReq,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(o => o.Id == updateAccountReq.Id,
                cancellationToken);
            if (account == null) throw new BusinessException("账户不存在");
            account.AccountName = updateAccountReq.AccountName;
            account.AccountType = updateAccountReq.AccountType;
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("更新账户 [{accountName}]", account.AccountName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DeleteAccountAsync(long id, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            Account? account =
                await _dbContext.Accounts.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (account == null) throw new BusinessException("账户不存在");
            _dbContext.Accounts.Remove(account);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("删除账户 [{accountName}]", account.AccountName);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}