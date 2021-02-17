using Application.Accounts;
using Application.Accounts.Models;
using Application.Orders;
using Application.Orders.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.IntegrationTests.Accounts
{
    using static Testing;

    public class GetAccountDetailsQueryTests : TestBase
    {
        [Test]
        public async Task ShouldReturnAccountDetails()
        {
            await RunAsTestUserAsync();

            var firstInvestment = 1000;
            var account = await CreateAccount(firstInvestment);
            var totalShares = 2;
            var sharePrice = 50;
            var buyOrder = await SendOrder(account.Id, "BUY", "AOL", totalShares, sharePrice);
            var sellOrder = await SendOrder(account.Id, "SELL", "AOL", totalShares, sharePrice);

            var currentBalance = firstInvestment;

            var accountDto = await SendAsync(new GetAccountDetailsQuery { AccountId = account.Id });

            accountDto.Should().NotBeNull();
            accountDto.Cash.Should().Be(currentBalance);
            accountDto.Orders.Count.Should().Be(2);
            accountDto.Stock.Where(x => x.IssuerName == "AOL").SingleOrDefault().Quantity.Should().Be(0);
        }

        private async Task<AccountResult> CreateAccount(decimal firstInvestment)
        {
            return await SendAsync(new CreateAccountCommand { Cash = firstInvestment });
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
