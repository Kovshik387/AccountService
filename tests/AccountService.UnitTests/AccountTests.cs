using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountService.Domain.Entities;
using AccountService.Domain.Entities.Enums;
using AccountService.Domain.Models;
using AccountService.Domain.Repositories;
using AccountService.Features.Accounts;
using AccountService.Features.Accounts.Commands.UpdateAccount;
using AccountService.Features.Exceptions;
using AccountService.UnitTests.Fakers;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AccountService.UnitTests;

public class AccountTests
{
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly Mock<ILogger<UpdateAccountHandler>> _logger = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public void UpdateAccountCommand_Should_MapToUpdateAccountModel()
    {
        // Arrange
        var config = new MapperConfiguration(x => { x.AddProfile<MappingProfile>(); },
            new NullLoggerFactory()
        );

        var mapper = config.CreateMapper();
        var command = UpdateAccountCommandFaker.Generate().First();

        // Act
        var model = mapper.Map<UpdateAccountModel>(command);

        // Assert
        model.Should().NotBeNull();
        model.Id.Should().Be(command.Id);
        model.InterestRate.Should().Be(command.InterestRate);
        model.XMin.Should().Be(command.XMin);
    }

    [Fact]
    public async Task UpdateAccountCommandWithValidXMin_Should_ReturnId()
    {
        // Arrange
        var command = UpdateAccountCommandFaker.Generate().First();
        var mapped = new UpdateAccountModel
        {
            Id = command.Id,
            InterestRate = command.InterestRate,
            XMin = command.XMin
        };

        _accountRepository.Setup(x => x.UpdateAccountAsync(mapped, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mapped.Id);

        // Act
        var result = await _accountRepository.Object.UpdateAccountAsync(mapped, CancellationToken.None);

        // Assert 
        result.Should().Be(mapped.Id);
    }

    [Fact]
    public async Task UpdateAccountCommand_Should_UpdateAndReturnId()
    {
        // Arrange
        var command = UpdateAccountCommandFaker.Generate().First();
        var mapped = new UpdateAccountModel
        {
            Id = command.Id,
            InterestRate = command.InterestRate,
            XMin = command.XMin
        };

        _accountRepository.Setup(r => r.UpdateAccountAsync(mapped, It.IsAny<CancellationToken>()))
            .ReturnsAsync(command.Id);
        _mapper.Setup(m => m.Map<UpdateAccountModel>(command)).Returns(mapped);
        var handler = new UpdateAccountHandler(_accountRepository.Object, _logger.Object, _mapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);

        _accountRepository.Verify(r => r.UpdateAccountAsync(mapped, It.IsAny<CancellationToken>()),
            Times.Once);
        _accountRepository.Verify(r => r.GetAccountByIdAsync(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAccountCommand_Should_ThrowNotFound()
    {
        // Arrange
        var command = UpdateAccountCommandFaker.Generate().First();
        var mapped = new UpdateAccountModel
        {
            Id = command.Id,
            InterestRate = command.InterestRate,
            XMin = command.XMin
        };
        _accountRepository.Setup(r => r.UpdateAccountAsync(mapped, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _accountRepository.Setup(r => r.GetAccountByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        var handler = new UpdateAccountHandler(_accountRepository.Object, _logger.Object, _mapper.Object);

        // Act
        var func = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await func.Should().ThrowAsync<AccountNotFoundException>();
        _accountRepository.Verify(r => r.GetAccountByIdAsync(command.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAccountCommand_Should_ThrowConflict()
    {
        // Arrange
        var command = UpdateAccountCommandFaker.Generate().First();
        var mapped = new UpdateAccountModel
        {
            Id = command.Id,
            InterestRate = command.InterestRate,
            XMin = command.XMin
        };
        var existing = new Account
        {
            Id = command.Id,
            OwnerId = Guid.NewGuid(),
            Currency = "USD",
            Balance = 100m,
            Type = AccountType.Deposit,
            OpeningDate = DateTimeOffset.UtcNow
        };

        _mapper.Setup(m => m.Map<UpdateAccountModel>(command)).Returns(mapped);
        _accountRepository.Setup(r => r.UpdateAccountAsync(mapped, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _accountRepository.Setup(r => r.GetAccountByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = new UpdateAccountHandler(_accountRepository.Object, _logger.Object, _mapper.Object);

        // Act
        var func = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await func.Should().ThrowAsync<ConflictException>();

        _accountRepository.Verify(r => r.UpdateAccountAsync(mapped, It.IsAny<CancellationToken>()),
            Times.Once);
        _accountRepository.Verify(r => r.GetAccountByIdAsync(command.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        _accountRepository.VerifyNoOtherCalls();
    }
}