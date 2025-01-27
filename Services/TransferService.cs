using AutoMapper;
using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gacfox.Wealthome.Services;

public class TransferService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginService> _logger;

    public TransferService(AppDbContext dbContext, IMapper mapper, ILogger<LoginService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    private TransferDto BuildTransferDto(Transfer transfer)
    {
        TransferDto transferDto = _mapper.Map<TransferDto>(transfer)!;
        transferDto.TransferTypeDto = _mapper.Map<TransferTypeDto>(transfer.TransferType)!;
        List<Flow> flows = transfer.Flows;
        List<FlowDto> flowDtos = new List<FlowDto>();
        foreach (Flow flow in flows)
        {
            FlowDto flowDto = _mapper.Map<FlowDto>(flow)!;
            flowDto.AccountDto = _mapper.Map<AccountDto>(flow.Account)!;
            User accountUser = flow.Account.User;
            UserDto accountUserDto = _mapper.Map<UserDto>(accountUser)!;
            flowDto.AccountDto.UserDto = accountUserDto;
            flowDtos.Add(flowDto);
        }

        User transferUser = transfer.User;
        UserDto? transferUserDto = _mapper.Map<UserDto>(transferUser);
        transferDto.UserDto = transferUserDto!;

        transferDto.FlowDtos = flowDtos;
        return transferDto;
    }

    public Pagination<TransferDto?> GetTransferList(QueryTransferReq queryTransferReq)
    {
        IQueryable<Transfer> queryable = _dbContext.Transfers.AsQueryable();
        queryable = queryable.Include(o => o.TransferType).Include(o => o.User);
        queryable = queryable.Include(o => o.Flows)
            .ThenInclude(o => o.Account)
            .ThenInclude(o => o.User);
        if (!string.IsNullOrEmpty(queryTransferReq.Pattern))
        {
            queryable = queryable.Where(o =>
                o.Title.Contains(queryTransferReq.Pattern) || (!string.IsNullOrEmpty(o.Description) &&
                                                               o.Description.Contains(queryTransferReq.Pattern)));
        }

        if (queryTransferReq.TransferUserId.HasValue)
        {
            queryable = queryable.Where(o => o.UserId == queryTransferReq.TransferUserId);
        }

        if (queryTransferReq.TransferTypeId.HasValue)
        {
            queryable = queryable.Where(o => o.TransferTypeId == queryTransferReq.TransferTypeId);
        }

        if (queryTransferReq.IncomeAccountId.HasValue)
        {
            queryable = queryable.Where(o =>
                o.Flows.Any(p => p.AccountId == queryTransferReq.IncomeAccountId && p.Amount > 0));
        }

        if (queryTransferReq.ExpenseAccountId.HasValue)
        {
            queryable = queryable.Where(o =>
                o.Flows.Any(p => p.AccountId == queryTransferReq.ExpenseAccountId && p.Amount < 0));
        }

        if (queryTransferReq.IncomeUserId.HasValue)
        {
            queryable = queryable.Where(o =>
                o.Flows.Any(p => p.Account.UserId == queryTransferReq.IncomeUserId && p.Amount > 0));
        }

        if (queryTransferReq.ExpenseUserId.HasValue)
        {
            queryable = queryable.Where(o =>
                o.Flows.Any(p => p.Account.UserId == queryTransferReq.ExpenseUserId && p.Amount < 0));
        }

        if (!string.IsNullOrEmpty(queryTransferReq.FromDate))
        {
            DateTime fromDateTime = DateTime.Parse(queryTransferReq.FromDate);
            queryable = queryable.Where(o => o.CreatedAt >= fromDateTime);
        }

        if (!string.IsNullOrEmpty(queryTransferReq.ToDate))
        {
            var toDate = DateTime.Parse(queryTransferReq.ToDate).AddDays(1).AddTicks(-1);
            queryable = queryable.Where(t => t.CreatedAt <= toDate);
        }

        int total = queryable.Count();
        List<Transfer> transfers = queryable.OrderByDescending(o => o.CreatedAt)
            .Skip((queryTransferReq.PageNum - 1) * queryTransferReq.PageSize)
            .Take(queryTransferReq.PageSize).ToList();

        List<TransferDto?> transferDtos = new List<TransferDto?>();
        foreach (Transfer transfer in transfers)
        {
            transferDtos.Add(BuildTransferDto(transfer));
        }

        return new Pagination<TransferDto?>
        {
            List = transferDtos,
            Total = total,
            TotalPage = (int)Math.Ceiling((double)total / queryTransferReq.PageSize),
            Current = queryTransferReq.PageNum,
            PageSize = queryTransferReq.PageSize
        };
    }

    public TransferDto? GetTransferById(long id)
    {
        IQueryable<Transfer> queryable = _dbContext.Transfers.AsQueryable();
        queryable = queryable.Include(o => o.TransferType).Include(o => o.User);
        queryable = queryable.Include(o => o.Flows)
            .ThenInclude(o => o.Account)
            .ThenInclude(o => o.User);
        Transfer? transfer = queryable.FirstOrDefault(o => o.Id == id);
        return transfer == null ? null : BuildTransferDto(transfer);
    }

    public void CreateTransfer(CreateTransferReq createTransferReq, string currentLoginUserName)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            User? transferUser = _dbContext.Users.FirstOrDefault(o =>
                o.UserName == currentLoginUserName && o.Status != User.UserStatusDeleted);
            if (transferUser == null) throw new BusinessException("当前登录用户不存在");

            TransferType? transferType =
                _dbContext.TransferTypes.FirstOrDefault(o => o.Id == createTransferReq.TransferTypeId);
            if (transferType == null) throw new BusinessException("交易类别不存在");

            Account? fromAccount = _dbContext.Accounts.Include(o => o.User)
                .FirstOrDefault(o => o.Id == createTransferReq.FromAccountId);
            if (fromAccount == null) throw new BusinessException("转出账户不存在");

            Account? toAccount = _dbContext.Accounts.Include(o => o.User)
                .FirstOrDefault(o => o.Id == createTransferReq.ToAccountId);
            if (toAccount == null) throw new BusinessException("转入账户不存在");

            DateTime createdAt = DateTime.Parse(createTransferReq.CreatedAtStr);

            Flow incomeFlow = new Flow
            {
                AccountId = createTransferReq.ToAccountId,
                Account = toAccount,
                Amount = decimal.Parse(createTransferReq.AmountStr),
                CreatedAt = createdAt
            };

            Flow expenseFlow = new Flow
            {
                AccountId = createTransferReq.FromAccountId,
                Account = fromAccount,
                Amount = -decimal.Parse(createTransferReq.AmountStr),
                CreatedAt = createdAt
            };

            toAccount.Balance += decimal.Parse(createTransferReq.AmountStr);
            fromAccount.Balance -= decimal.Parse(createTransferReq.AmountStr);

            Transfer transfer = new Transfer
            {
                Title = createTransferReq.Title,
                TransferTypeId = createTransferReq.TransferTypeId,
                TransferType = transferType,
                Description = createTransferReq.Description,
                UserId = transferUser.Id,
                User = transferUser,
                Flows = new List<Flow> { incomeFlow, expenseFlow },
                CreatedAt = createdAt
            };

            _dbContext.Flows.Add(incomeFlow);
            _dbContext.Flows.Add(expenseFlow);
            _dbContext.Transfers.Add(transfer);
            _dbContext.Accounts.Update(fromAccount);
            _dbContext.Accounts.Update(toAccount);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("新增交易 [{title}]", transfer.Title);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void RevertTransfer(RevertTransferReq revertTransferReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            IQueryable<Transfer> queryable = _dbContext.Transfers.AsQueryable();
            queryable = queryable.Include(o => o.Flows).ThenInclude(o => o.Account).ThenInclude(o => o.User);
            Transfer? transfer = queryable.FirstOrDefault(o => o.Id == revertTransferReq.Id);
            if (transfer == null) throw new BusinessException("交易不存在");
            List<Flow> flows = transfer.Flows;
            foreach (Flow flow in flows)
            {
                Account account = flow.Account;
                account.Balance -= flow.Amount;
                _dbContext.Accounts.Update(account);
            }

            _dbContext.Transfers.Remove(transfer);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("撤销交易 [{title}]", transfer.Title);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateTransfer(UpdateTransferReq updateTransferReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            Transfer? transfer = _dbContext.Transfers.Include(o => o.Flows)
                .FirstOrDefault(o => o.Id == updateTransferReq.Id);
            if (transfer == null) throw new BusinessException("交易不存在");
            transfer.Title = updateTransferReq.Title;
            transfer.CreatedAt = DateTime.Parse(updateTransferReq.CreatedAtStr);
            transfer.Description = updateTransferReq.Description;

            TransferType? transferType =
                _dbContext.TransferTypes.FirstOrDefault(o => o.Id == updateTransferReq.TransferTypeId);
            if (transferType != null)
            {
                transfer.TransferTypeId = updateTransferReq.TransferTypeId;
                transfer.TransferType = transferType;
            }

            _dbContext.Transfers.Update(transfer);

            foreach (Flow flow in transfer.Flows)
            {
                flow.CreatedAt = transfer.CreatedAt;
                _dbContext.Flows.Update(flow);
            }

            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("更新交易 [{title}]", transfer.Title);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}