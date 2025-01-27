using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
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

    public List<TransferTypeDto> GetTransferTypeList()
    {
        List<TransferType> transferTypes = _dbContext.TransferTypes.ToList();
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

    public TransferTypeDto? GetTransferTypeById(long id)
    {
        TransferType? transferType = _dbContext.TransferTypes.FirstOrDefault(o => o.Id == id);
        return transferType != null
            ? new TransferTypeDto
            {
                Id = transferType.Id,
                Name = transferType.Name,
                InoutType = transferType.InoutType
            }
            : null;
    }

    public void CreateTransferType(CreateTransferTypeReq createTransferTypeReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            TransferType transferType = new TransferType
            {
                Name = createTransferTypeReq.Name,
                InoutType = createTransferTypeReq.InoutType
            };
            _dbContext.TransferTypes.Add(transferType);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("新增交易类别 [{name}]", transferType.Name);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateTransferType(UpdateTransferTypeReq updateTransferTypeReq)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            TransferType? transferType =
                _dbContext.TransferTypes.FirstOrDefault(o => o.Id == updateTransferTypeReq.Id);
            if (transferType == null)
            {
                throw new BusinessException("交易类别不存在");
            }

            transferType.Name = updateTransferTypeReq.Name;
            transferType.InoutType = updateTransferTypeReq.InoutType;
            _dbContext.TransferTypes.Update(transferType);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("更新交易类别 [{name}]", transferType.Name);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void DeleteTransferType(long id)
    {
        using IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
        try
        {
            TransferType? transferType = _dbContext.TransferTypes.FirstOrDefault(o => o.Id == id);
            if (transferType == null) throw new BusinessException("交易类别不存在");

            if (_dbContext.Transfers.Any(o => o.TransferTypeId == id))
            {
                throw new BusinessException("该交易类别存在关联交易数据, 无法删除");
            }

            _dbContext.TransferTypes.Remove(transferType);
            _dbContext.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("删除交易类别 [{name}]", transferType.Name);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}