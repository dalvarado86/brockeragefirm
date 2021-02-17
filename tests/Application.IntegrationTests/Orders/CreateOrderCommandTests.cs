using Application.Accounts;
using Application.Accounts.Models;
using Application.Orders;
using Application.Orders.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.IntegrationTests.Orders
{
    using static Testing;

    public class CreateOrderCommandTests : TestBase
    {        
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new CreateOrderCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<Exception>();
        }

        [Test]
        public async Task ShouldCreateBuyOperation()
        {
            await RunAsTestUserAsync();

            var firstInvestment = 1000;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 2;
            var sharePrice = 50;
            var order = await SendOrder(account.Id, "BUY", "AAPL", totalShares, sharePrice);

            var currentBalance = firstInvestment - (totalShares * sharePrice);
         
            order.Should().NotBeNull();
            order.CurrentBalance.Cash.Should().Be(currentBalance);
            order.CurrentBalance.Issuers.Count.Should().Be(1);
            order.BusinessErrors.Count.Should().Be(0);
        }

        [Test]
        public async Task ShouldCreateSellOperation()
        {
            await RunAsTestUserAsync();

            var firstInvestment = 1000;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 2;
            var sharePrice = 50;
            var buyOrder = await SendOrder(account.Id, "BUY", "AOL", totalShares, sharePrice);
            var sellOrder = await SendOrder(account.Id, "SELL", "AOL", totalShares, sharePrice);

            var currentBalance = firstInvestment;

            sellOrder.Should().NotBeNull();
            sellOrder.CurrentBalance.Cash.Should().Be(currentBalance);
            sellOrder.CurrentBalance.Issuers.Count.Should().Be(2);
            sellOrder.BusinessErrors.Count.Should().Be(0);
        }

        [Test]
        public async Task ShouldCreateInvalidOperation()
        {
            await RunAsTestUserAsync();

            var firstInvestment = 1000;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 2;
            var sharePrice = 50;
            var order = await SendOrder(account.Id, "GIVE", "AAPL", totalShares, sharePrice);

            order.Should().NotBeNull();
            order.CurrentBalance.Cash.Should().Be(firstInvestment);
            order.CurrentBalance.Issuers.Count.Should().Be(0);
            order.BusinessErrors.Contains("INVALID_OPERATION").Should().Be(true);
        }

        [Test]
        public async Task ShouldBeInsufficientFunds()
        {
             await RunAsTestUserAsync();

            var firstInvestment = 1;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 2;
            var sharePrice = 50;
            var order = await SendOrder(account.Id, "BUY", "AAPL", totalShares, sharePrice);

            order.Should().NotBeNull();
            order.CurrentBalance.Cash.Should().Be(firstInvestment);
            order.CurrentBalance.Issuers.Count.Should().Be(0);
            order.BusinessErrors.Contains("INSUFFICIENT_BALANCE").Should().Be(true);
        }

        [Test]
        public async Task ShouldBeInsufficientStock()
        {
            await RunAsTestUserAsync();

            var firstInvestment = 1000;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 10;
            var sharePrice = 80;
            var sellOrder = await SendOrder(account.Id, "SELL", "NTFX", totalShares, sharePrice);

            sellOrder.Should().NotBeNull();
            sellOrder.CurrentBalance.Cash.Should().Be(firstInvestment);
            sellOrder.CurrentBalance.Issuers.Count.Should().Be(0);
            sellOrder.BusinessErrors.Contains("INSUFFICIENT_STOCKS").Should().Be(true);
        }

        [Test]
        public async Task ShouldMarkedIsClosed()
        {
            // TODO: Mock service to rewirite MarketSettings
            await RunAsTestUserAsync();

            var firstInvestment = 1000;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 10;
            var sharePrice = 80;
            var sellOrder = await SendOrder(account.Id, "SELL", "NTFX", totalShares, sharePrice);

            sellOrder.Should().NotBeNull();
            sellOrder.CurrentBalance.Cash.Should().Be(firstInvestment);
            sellOrder.CurrentBalance.Issuers.Count.Should().Be(0);
            sellOrder.BusinessErrors.Contains("CLOSED_MARKET").Should().Be(true);
        }


        private async Task<AccountResult> CreateAccount(decimal firstInvestment)
        {
            return await SendAsync(new CreateAccountCommand { Cash = firstInvestment } );
        }

        private async Task<OrderResult> SendOrder(int accountId, string operation, string issuerName, int totalShares, decimal sharePrices)
        {
            var orderCommand = new CreateOrderCommand
            {
                AccountId = accountId,
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Operation = operation,
                IssuerName = issuerName,
                TotalShares = totalShares,
                SharePrice = sharePrices
            };

            return await SendAsync(orderCommand);
        }

    }
}
