using AccountService.Domain.Entities;
using AccountService.Domain.Models;
using AccountService.Features.Transactions.Commands.CreateTransaction;
using AccountService.Features.Transactions.Queries.GetTransactionById;
using AutoMapper;

namespace AccountService.Features.Transactions;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetTransactionByIdQueryResponse, Transaction>().ReverseMap();
        CreateMap<TransactionCompleteModel, CreateTransactionResponse>().ReverseMap();
    }
}