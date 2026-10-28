using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gacfox.Wealthome.Services;

public class TransferTypeService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TransferTypeService> _logger;

    public TransferTypeService(AppDbContext dbContext, ILogger<TransferTypeService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<TransferTypeDto>> GetTransferTypeListAsync(
        CancellationToken cancellationToken = default)
    {
        List<TransferType> transferTypes = await _dbContext.TransferTypes.ToListAsync(cancellationToken);
        List<TransferTypeDto> transferTypeDtos = new List<TransferTypeDto>();
        foreach (TransferType transferType in transferTypes)
        {
            transferTypeDtos.Add(new TransferTypeDto
            {
                Id = transferType.Id,
                Name = transferType.Name,
                InoutType = transferType.InoutType
            });
        }

        return transferTypeDtos;
    }

    public async Task<TransferTypeDto?> GetTransferTypeByIdAsync(long id,
        CancellationToken cancellationToken = default)
    {
        TransferType? transferType =
            await _dbContext.TransferTypes.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        return transferType != null
            ? new TransferTypeDto
            {
                Id = transferType.Id,
                Name = transferType.Name,
                InoutType = transferType.InoutType
            }
            : null;
    }

    public async Task CreateTransferTypeAsync(CreateTransferTypeReq createTransferTypeReq,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            TransferType transferType = new TransferType
            {
                Name = createTransferTypeReq.Name,
                InoutType = createTransferTypeReq.InoutType
            };
            _dbContext.TransferTypes.Add(transferType);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("新增交易类别 [{name}]", transferType.Name);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task UpdateTransferTypeAsync(UpdateTransferTypeReq updateTransferTypeReq,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            TransferType? transferType =
                await _dbContext.TransferTypes.FirstOrDefaultAsync(o => o.Id == updateTransferTypeReq.Id,
                    cancellationToken);
            if (transferType == null)
            {
                throw new BusinessException("交易类别不存在");
            }

            transferType.Name = updateTransferTypeReq.Name;
            transferType.InoutType = updateTransferTypeReq.InoutType;
            _dbContext.TransferTypes.Update(transferType);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("更新交易类别 [{name}]", transferType.Name);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DeleteTransferTypeAsync(long id, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            TransferType? transferType =
                await _dbContext.TransferTypes.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (transferType == null) throw new BusinessException("交易类别不存在");

            if (await _dbContext.Transfers.AnyAsync(o => o.TransferTypeId == id, cancellationToken))
            {
                throw new BusinessException("该交易类别存在关联交易数据, 无法删除");
            }

            _dbContext.TransferTypes.Remove(transferType);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("删除交易类别 [{name}]", transferType.Name);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}