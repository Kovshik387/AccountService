using AccountService.Domain.Entities;
using AccountService.Domain.Models;
using AccountService.Features.Accounts.Commands.CreateAccount;
using AccountService.Features.Accounts.Commands.UpdateAccount;
using AccountService.Features.Accounts.Queries.GetAccountById;
using AccountService.Features.Accounts.Queries.GetAccountDetailsById;
using AccountService.Features.Accounts.Queries.GetAccountsByOwnerId;
using AutoMapper;

namespace AccountService.Features.Accounts;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAccountCommand, Account>().ReverseMap();
        CreateMap<GetAccountListByOwnerIdQueryResponse, Account>().ReverseMap();
        CreateMap<Account, GetAccountByIdQueryResponse>().ReverseMap();
        CreateMap<UpdateAccountCommand , UpdateAccountModel>().ReverseMap();
        CreateMap<Account, GetAccountDetailsByIdQueryResponse>().ReverseMap();
    }
}