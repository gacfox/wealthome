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

    public List<AccountDto?> GetAccountList()
    {
        List<AccountDto?> accountDtos = new List<AccountDto?>();
        List<Account> accounts = _dbContext.Accounts.Include(o => o.User).OrderByDescending(o => o.CreatedAt).ToList();
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

    public AccountDto? GetAccountById(long id)
    {
        Account? account = _dbContext.Accounts.Include(o => o.User).FirstOrDefault(o => o.Id == id);
        AccountDto? accountDto = _mapper.Map<AccountDto>(account);
        if (account == null || accountDto == null) return null;
        User u = account.User;
        UserDto? userDto = _mapper.Map<UserDto>(u);
        accountDto.UserDto = userDto;
        return accountDto;
    }

    public void CreateAccount(CreateAccountReq createAccountReq, string currentLoginUserName)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? user = _dbContext.Users.FirstOrDefault(o => o.UserName == currentLoginUserName);
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
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("新增账户 [{accountName}]", account.AccountName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateAccount(UpdateAccountReq updateAccountReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            Account? account = _dbContext.Accounts.FirstOrDefault(o => o.Id == updateAccountReq.Id);
            if (account == null) throw new BusinessException("账户不存在");
            account.AccountName = updateAccountReq.AccountName;
            account.AccountType = updateAccountReq.AccountType;
            _dbContext.Accounts.Update(account);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("更新账户 [{accountName}]", account.AccountName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void DeleteAccount(long id)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            Account? account = _dbContext.Accounts.FirstOrDefault(o => o.Id == id);
            if (account == null) throw new BusinessException("账户不存在");
            _dbContext.Accounts.Remove(account);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("删除账户 [{accountName}]", account.AccountName);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}