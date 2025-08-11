using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountService.Domain.Entities;
using AccountService.Domain.Repositories;
using AccountService.Features.Exceptions;
using AccountService.Features.Transactions;
using AccountService.Features.Transactions.Queries.GetTransactionById;
using AccountService.UnitTests.Fakers;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountService.UnitTests;

public class TransactionTests
{
    [Fact]
    public void TransferTransactionCommand_Should_MapToTransaction()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); },
            new LoggerFactory());
        var mapper = config.CreateMapper();

        var command = TransactionFaker.Generate().First();

        // Act
        var transaction = mapper.Map<Transaction>(command);

        // Assert
        transaction.Should().NotBeNull();
        transaction.AccountId.Should().Be(command.AccountId);
        transaction.Amount.Should().Be(command.Amount);
        transaction.Currency.Should().Be(command.Currency);
        transaction.Description.Should().Be(command.Description);

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public async Task GetTransactionByIdQuery_Should_ThrowNotFound()
    {
        // Arrange
        var repository = new Mock<ITransactionRepository>();
        var mapper = new Mock<IMapper>();

        var handler = new GetTransactionByIdQueryHandler(repository.Object, mapper.Object);

        var query = new GetTransactionByIdQuery
        {
            Id = Guid.NewGuid()
        };

        repository.Setup(r => r.GetTransactionByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction)null);

        // Act
        var func = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await func.Should().ThrowAsync<TransactionNotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_Return_Mapped_Response_When_Found()
    {
        // Arrange
        var repo = new Mock<ITransactionRepository>();
        var mapper = new Mock<IMapper>();

        var transaction = TransactionFaker.Generate().First();

        var expected = new GetTransactionByIdQueryResponse
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            CounterpartyAccountId = transaction.CounterpartyAccountId,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Type = transaction.Type,
            Description = transaction.Description,
            Date = transaction.Date
        };

        var query = new GetTransactionByIdQuery
        {
            Id = transaction.Id
        };

        repo.Setup(r => r.GetTransactionByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        mapper.Setup(m => m.Map<GetTransactionByIdQueryResponse>(transaction))
            .Returns(expected);

        var handler = new GetTransactionByIdQueryHandler(repo.Object, mapper.Object);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}