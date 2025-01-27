using AutoMapper;
using Gacfox.Wealthome.Models;

namespace Gacfox.Wealthome;

public class AutoMappingProfile : Profile
{
    public AutoMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Account, AccountDto>();
        CreateMap<Transfer, TransferDto>();
        CreateMap<Flow, FlowDto>();
        CreateMap<TransferType, TransferTypeDto>();
    }
}